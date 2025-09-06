using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using Owin;


[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }

    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
// Configure logging using NLog
            NLog.LogManager.GetCurrentClassLogger().Info("Logging configured successfully");
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configure application settings
            System.Console.WriteLine("Configuration setup completed");
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Configure dependency injection
            var builder = new Autofac.ContainerBuilder();

            // Register dependencies

            var container = builder.Build();

            // Use Autofac with OWIN
            app.UseAutofacMiddleware(container);
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Configure authentication
            app.UseOpenIdConnectAuthentication(new Microsoft.Owin.Security.OpenIdConnect.OpenIdConnectAuthenticationOptions
            {
                ClientId = "your-client-id",
                Authority = "https://your-authority",
                RedirectUri = "https://your-redirect-uri",
                PostLogoutRedirectUri = "https://your-post-logout-redirect-uri",
                Scope = "openid profile",
                ResponseType = "code id_token",
                UseTokenLifetime = true
            });
        }
    }
}