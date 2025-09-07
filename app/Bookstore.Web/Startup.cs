using Microsoft.Owin;
using Owin;
using NLog;


[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            // Initialize logging configuration
            LogManager.LoadConfiguration("NLog.config");
        }
    }

    public class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configuration setup code
        }
    }

    public class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Dependency injection setup code
        }
    }

    public class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Authentication setup code
        }
    }

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
}
