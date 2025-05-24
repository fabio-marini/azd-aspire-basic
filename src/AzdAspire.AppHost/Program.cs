using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// TODO: runtime vs publish?

var app1 = builder.AddProject<Projects.AzdAspire_WebApplication1>("webapp1");
var app2 = builder.AddProject<Projects.AzdAspire_WebApplication2>("webapp2");

if (builder.Environment.IsEnvironment("Development"))
{
    var rmq = builder.AddRabbitMQ("rmq-messaging");

    app1.WithReference(rmq);
    app2.WithReference(rmq);
}
else
{
    // both hybrid and production environments use these Azure resources
    var asb = builder.AddAzureServiceBus("asb-messaging");
    var akv = builder.AddAzureKeyVault("app-secrets");
    var cfg = builder.AddAzureAppConfiguration("app-config");

    app1.WithReference(asb).WithReference(akv).WithReference(cfg);
    app2.WithReference(asb).WithReference(akv).WithReference(cfg);
}

var stg = builder.AddAzureStorage("messages-stg").RunAsEmulator();
var tbl = stg.AddTables("messages-tbl");
var blb = stg.AddBlobs("messages-blb");

// all environments use table and blob storage
app1.WithReference(tbl).WithReference(blb);
app2.WithReference(tbl).WithReference(blb);

builder.Build().Run();
