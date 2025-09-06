using Amazon.CDK;

namespace Bookstore.Cdk;

internal sealed class Program
{
    public static void Main()
    {
        var app = new App();

        var env = MakeEnv();

        var appName = "Bookstore"; // Replace with actual app name
        var coreStack = new CoreStack(app, $"{appName}Core", new StackProps { Env = env });
        var networkStack = new NetworkStack(app, $"{appName}Network", new StackProps { Env = env });
        var databaseStack = new DatabaseStack(app, $"{appName}Database", new DatabaseStackProps { Env = env, Vpc = networkStack.Vpc });
        var ecsStack = new EcsStack(app, $"{appName}ECS", new EcsStackProps { Env = env, Vpc = networkStack.Vpc, Database = databaseStack.Database, ImageBucket = coreStack.ImageBucket, WebAppUserPool = coreStack.WebAppUserPool });

        app.Synth();
    }

    private static Environment MakeEnv(string account = null, string region = null)
    {
        return new Environment
        {
            Account = account ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
            Region = region ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
        };
    }
}