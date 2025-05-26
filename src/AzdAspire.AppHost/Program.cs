using Aspire.Hosting.Azure;
using DotNetEnv.Configuration;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// TODO: refactor consts from extensions - how to share?
// TODO: remove storage all together, if not used + akv and app config aren't really used either...
// TODO: use hooks to add secret and key/value pair + retrieve in message handlers to prove akv/app config
// TODO: test local and hybrid + integration (expose HTTP endpoint)

builder.Configuration.AddDotNetEnv($"../../.azure/{builder.Environment.EnvironmentName}/.env");

var app1 = builder.AddProject<Projects.AzdAspire_WebApplication1>("webapp1");
var app2 = builder.AddProject<Projects.AzdAspire_WebApplication2>("webapp2");

if (builder.Environment.IsDevelopment())
{
    var configureEmulator = new Action<IResourceBuilder<AzureStorageEmulatorResource>>(bld =>
    {
        bld.WithBlobPort(58522);
        bld.WithQueuePort(58523);
        bld.WithTablePort(58524);
    });

    var stg = builder.GetStorageResource("messages-stg").RunAsEmulator(configureEmulator);

    var tbl = stg.AddTables("messages-tbl");
    var blb = stg.AddBlobs("messages-blb");

    var rmq = builder.AddRabbitMQ("rmq-messaging");

    app1.WithReference(rmq).WaitFor(rmq).WithReference(tbl).WithReference(blb).WaitFor(stg);
    app2.WithReference(rmq).WaitFor(rmq).WithReference(tbl).WithReference(blb).WaitFor(stg);
}
else
{
    // both hybrid and production environments use these Azure resources
    var akv = builder.AddFromConfiguration("app-secrets", "APP_SECRETS_VAULTURI") ?? builder.AddAzureKeyVault("app-secrets");
    var asb = builder.AddFromConfiguration("asb-messaging", "ASB_MESSAGING_SERVICEBUSENDPOINT") ?? builder.AddAzureServiceBus("asb-messaging");
    var cfg = builder.AddFromConfiguration("app-config", "APP_CONFIG_APPCONFIGENDPOINT") ?? builder.AddAzureAppConfiguration("app-config");
    var tbl = builder.AddFromConfiguration("messages-tbl", "MESSAGES_STG_TABLEENDPOINT") ?? builder.GetStorageResource("messages-stg").AddTables("messages-tbl");
    var blb = builder.AddFromConfiguration("messages-blb", "MESSAGES_STG_BLOBENDPOINT") ?? builder.GetStorageResource("messages-stg").AddBlobs("messages-blb");

    app1.WithReference(asb).WithReference(akv).WithReference(cfg).WithReference(tbl).WithReference(blb);
    app2.WithReference(asb).WithReference(akv).WithReference(cfg).WithReference(tbl).WithReference(blb);
}

builder.Build().Run();
