using DotNetEnv.Configuration;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddDotNetEnv($"../../.azure/{builder.Environment.EnvironmentName}/.env");

var app1 = builder.AddProject<Projects.AzdAspire_WebApplication1>("webapp1");
var app2 = builder.AddProject<Projects.AzdAspire_WebApplication2>("webapp2");

if (builder.Environment.IsDevelopment())
{
    var rmq = builder.AddRabbitMQ("rmq-messaging").WithManagementPlugin();

    app1.WithReference(rmq).WaitFor(rmq);
    app2.WithReference(rmq).WaitFor(rmq);
}
else
{
    // both hybrid and production environments use these Azure resources
    var akv = builder.AddFromConfiguration("app-secrets", "APP_SECRETS_VAULTURI")
        ?? builder.AddAzureKeyVault("app-secrets")
        ;

    var asb = builder.AddFromConfiguration("asb-messaging", "ASB_MESSAGING_SERVICEBUSENDPOINT") 
        ?? builder.AddAzureServiceBus("asb-messaging")
        ;

    var cfg = builder.AddFromConfiguration("app-config", "APP_CONFIG_APPCONFIGENDPOINT") 
        ?? builder.AddAzureAppConfiguration("app-config")
        ;

    app1.WithReference(asb).WithReference(akv).WithReference(cfg);
    app2.WithReference(asb).WithReference(akv).WithReference(cfg);
}

builder.Build().Run();
