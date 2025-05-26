using AzdAspire.ServiceDefaults;
using Azure.Identity;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport;
using IRouter = Rebus.Routing.IRouter;

public static class Extensions
{
    public static DefaultAzureCredential WithAzureCredentials(this IHostApplicationBuilder builder)
    {
        var credentialOptions = new DefaultAzureCredentialOptions
        {
            // leaves environment, MI (excluding in dev environment) and VS credentials - in that order
            // https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet
            ExcludeWorkloadIdentityCredential = true,
            ExcludeSharedTokenCacheCredential = true,

            // https://learn.microsoft.com/en-gb/dotnet/azure/sdk/authentication/credential-chains?tabs=dac#usage-guidance-for-defaultazurecredential
            ExcludeManagedIdentityCredential = builder.Environment.IsDevelopment()
        };

        return new DefaultAzureCredential(credentialOptions);
    }

    public static IHostApplicationBuilder AddAzureKeyVault(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("app-secrets");

        if (!String.IsNullOrEmpty(connectionString))
        {
            builder.Configuration.AddAzureKeyVault(new Uri(connectionString), builder.WithAzureCredentials());
        }

        return builder;
    }

    public static IHostApplicationBuilder AddAzureAppConfiguration(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("app-config");

        if (!String.IsNullOrEmpty(connectionString))
        {
            builder.Configuration.AddAzureAppConfiguration(opt =>
            {
                opt.Connect(new Uri(connectionString), builder.WithAzureCredentials());
            });

            builder.Services.AddAzureAppConfiguration();
        }

        return builder;
    }

    public static IHostApplicationBuilder AddRebus(this IHostApplicationBuilder builder)
    {
        var configureTransport = new Action<StandardConfigurer<ITransport>>(cfg =>
        {
            if (builder.Configuration.GetConnectionString("asb-messaging") is string asb && !String.IsNullOrEmpty(asb))
            {
                cfg.UseAzureServiceBus($"Endpoint={asb}", "rebus-webapp1", builder.WithAzureCredentials());
            }
            else if (builder.Configuration.GetConnectionString("rmq-messaging") is string rmq && !String.IsNullOrEmpty(rmq))
            {
                cfg.UseRabbitMq(rmq, "rebus-webapp1");
            }
        });

        var configureRouting = new Action<StandardConfigurer<IRouter>>(cfg =>
        {
            cfg.TypeBased().Map<EchoRequest>("rebus-webapp2");
        });

        builder.Services.AddRebus(cfg => cfg.Transport(configureTransport).Routing(configureRouting));

        builder.Services.AddRebusHandler<EchoResponseHandler>();

        return builder;
    }
}
