using Amazon.Rekognition;
using Amazon.S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BobsBookstoreClassic.Data;
using Bookstore.Common;
using Bookstore.Data;
using Bookstore.Data.FileServices;
using Bookstore.Data.ImageResizeService;
using Bookstore.Data.ImageValidationServices;
using Bookstore.Data.Repositories;
using Bookstore.Domain;
using Bookstore.Domain.Addresses;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Offers;
using Bookstore.Domain.Orders;
using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NLog;
using NLog.AWS.Logger;
using NLog.Config;
using NLog.Targets;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. NLog.Web.AspNetCore logging setup
// ---------------------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Host.UseNLog();

ConfigureNLog(builder.Configuration);

// ---------------------------------------------------------------------------
// 2. Autofac DI via builder.Host.UseServiceProviderFactory
// ---------------------------------------------------------------------------
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Domain services
    containerBuilder.RegisterType<BookService>().As<IBookService>();
    containerBuilder.RegisterType<OrderService>().As<IOrderService>();
    containerBuilder.RegisterType<ReferenceDataService>().As<IReferenceDataService>();
    containerBuilder.RegisterType<OfferService>().As<IOfferService>();
    containerBuilder.RegisterType<CustomerService>().As<ICustomerService>();
    containerBuilder.RegisterType<AddressService>().As<IAddressService>();
    containerBuilder.RegisterType<ShoppingCartService>().As<IShoppingCartService>();
    containerBuilder.RegisterType<ImageResizeService>().As<IImageResizeService>();

    // Database context
    var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");
    containerBuilder.RegisterType<ApplicationDbContext>()
        .WithParameter("connectionString", connectionString!)
        .InstancePerLifetimeScope();

    // Repositories
    containerBuilder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
    containerBuilder.RegisterType<AddressRepository>().As<IAddressRepository>();
    containerBuilder.RegisterType<BookRepository>().As<IBookRepository>();
    containerBuilder.RegisterType<OfferRepository>().As<IOfferRepository>();
    containerBuilder.RegisterType<ShoppingCartRepository>().As<IShoppingCartRepository>();
    containerBuilder.RegisterType<OrderRepository>().As<IOrderRepository>();
    containerBuilder.RegisterType<ReferenceDataRepository>().As<IReferenceDataRepository>();

    // Generic paginated list
    containerBuilder.RegisterGeneric(typeof(PaginatedList<>)).As(typeof(IPaginatedList<>)).InstancePerLifetimeScope();

    // File service (S3 or local)
    var fileServiceSetting = builder.Configuration["Services:FileService"];
    if (string.Equals(fileServiceSetting, "aws", StringComparison.OrdinalIgnoreCase))
    {
        containerBuilder.RegisterType<AmazonS3Client>().As<IAmazonS3>();
        containerBuilder.RegisterType<S3FileService>().As<IFileService>();
    }
    else
    {
        var webRootPath = Path.Combine(builder.Environment.WebRootPath ?? builder.Environment.ContentRootPath, "Content");
        containerBuilder.RegisterInstance(new LocalFileService(webRootPath)).As<IFileService>();
    }

    // Image validation service (Rekognition or local)
    var imageValidationSetting = builder.Configuration["Services:ImageValidationService"];
    if (string.Equals(imageValidationSetting, "aws", StringComparison.OrdinalIgnoreCase))
    {
        containerBuilder.RegisterType<AmazonRekognitionClient>().As<IAmazonRekognition>();
        containerBuilder.RegisterType<RekognitionImageValidationService>().As<IImageValidationService>();
    }
    else
    {
        containerBuilder.RegisterType<LocalImageValidationService>().As<IImageValidationService>();
    }
});

// ---------------------------------------------------------------------------
// 3. Populate BookstoreConfiguration from appsettings.json
// ---------------------------------------------------------------------------
SeedBookstoreConfiguration(builder.Configuration);

// ---------------------------------------------------------------------------
// 4. Authentication with OpenIdConnect + Cookies for AWS Cognito
// ---------------------------------------------------------------------------
var authServiceSetting = builder.Configuration["Services:Authentication"];
if (string.Equals(authServiceSetting, "aws", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ClientId = builder.Configuration["Authentication:Cognito:LocalClientId"];
        options.MetadataAddress = builder.Configuration["Authentication:Cognito:MetadataAddress"]!;
        options.SaveTokens = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true
        };
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async context =>
            {
                var customerService = context.HttpContext.RequestServices.GetRequiredService<ICustomerService>();
                var identity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                if (identity != null)
                {
                    var dto = new CreateOrUpdateCustomerDto(
                        identity.FindFirst("sub")?.Value ?? identity.FindFirst("nameidentifier")?.Value ?? string.Empty,
                        identity.Name ?? string.Empty,
                        identity.FindFirst("given_name")?.Value ?? string.Empty,
                        identity.FindFirst("family_name")?.Value ?? string.Empty);
                    await customerService.CreateOrUpdateCustomerAsync(dto);
                }
            }
        };
    });
}
else
{
    // Local development: no real auth scheme, use local auth middleware
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie();
}

// ---------------------------------------------------------------------------
// 5. MVC services with global filters
// ---------------------------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Migrated from FilterConfig: global authorize filter
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ---------------------------------------------------------------------------
// 6. Middleware pipeline
// ---------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
    {
        var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (ex is not null)
        {
            var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Unhandled exception");
        }
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }));
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Local authentication middleware (when not using Cognito)
if (!string.Equals(authServiceSetting, "aws", StringComparison.OrdinalIgnoreCase))
{
    app.UseMiddleware<LocalAuthenticationMiddleware>();
}

// ---------------------------------------------------------------------------
// 7. Routing — area + default routes (migrated from RouteConfig / AreaRegistration)
// ---------------------------------------------------------------------------
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ---------------------------------------------------------------------------
// Helper: Configure NLog (migrated from LoggingSetup.ConfigureLogging)
// ---------------------------------------------------------------------------
static void ConfigureNLog(IConfiguration configuration)
{
    var config = new LoggingConfiguration();

    Target loggingTarget;

    var loggingService = configuration["Services:LoggingService"];
    if (string.Equals(loggingService, "aws", StringComparison.OrdinalIgnoreCase))
    {
        loggingTarget = new AWSTarget { LogGroup = Constants.AppName };
    }
    else
    {
        loggingTarget = new DebuggerTarget();
    }

    config.AddTarget("aws", loggingTarget);
    config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Info, loggingTarget));

    LogManager.Configuration = config;
}

// ---------------------------------------------------------------------------
// Helper: Seed BookstoreConfiguration from IConfiguration (migrated from ConfigurationSetup)
// ---------------------------------------------------------------------------
static void SeedBookstoreConfiguration(IConfiguration configuration)
{
    // Populate the legacy BookstoreConfiguration singleton from appsettings.json
    // so existing code that calls BookstoreConfiguration.Get(...) continues to work.
    var services = configuration.GetSection("Services");
    foreach (var child in services.GetChildren())
    {
        BookstoreConfiguration.Add($"Services/{child.Key}", child.Value ?? string.Empty);
    }

    var connectionString = configuration.GetConnectionString("BookstoreDatabaseConnection");
    if (!string.IsNullOrEmpty(connectionString))
    {
        BookstoreConfiguration.Add("ConnectionStrings/BookstoreDatabaseConnection", connectionString);
    }

    // Authentication settings
    var authSection = configuration.GetSection("Authentication:Cognito");
    foreach (var child in authSection.GetChildren())
    {
        BookstoreConfiguration.Add($"Authentication/{child.Key}", child.Value ?? string.Empty);
    }

    // File settings
    var filesSection = configuration.GetSection("Files");
    foreach (var child in filesSection.GetChildren())
    {
        BookstoreConfiguration.Add($"Files/{child.Key}", child.Value ?? string.Empty);
    }
}

// Make the implicit Program class accessible for integration tests
public partial class Program { }
