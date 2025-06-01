using Azure.Identity;
using Rebus.Config;
using Rebus.Transport;

/// <summary>
/// Provides extension methods for configuring Azure services and Rebus messaging in the application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Creates a <see cref="DefaultAzureCredential"/> with options tailored for the application's environment.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <returns>A configured <see cref="DefaultAzureCredential"/> instance.</returns>
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

    /// <summary>
    /// Adds Azure Key Vault configuration to the application if a connection string is present.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IHostApplicationBuilder AddAzureKeyVault(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("app-secrets");

        if (!String.IsNullOrEmpty(connectionString))
        {
            builder.Configuration.AddAzureKeyVault(new Uri(connectionString), builder.WithAzureCredentials());
        }

        return builder;
    }

    /// <summary>
    /// Adds Azure App Configuration to the application if a connection string is present.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <returns>The application builder for chaining.</returns>
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

    /// <summary>
    /// Configures Rebus messaging for the application, using Azure Service Bus or RabbitMQ based on configuration.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IHostApplicationBuilder AddRebus(this IHostApplicationBuilder builder)
    {
        var configureTransport = new Action<StandardConfigurer<ITransport>>(cfg =>
        {
            if (builder.Configuration.GetConnectionString("asb-messaging") is string asb && !String.IsNullOrEmpty(asb))
            {
                cfg.UseAzureServiceBus($"Endpoint={asb}", "rebus-webapp2", builder.WithAzureCredentials());
            }
            else if (builder.Configuration.GetConnectionString("rmq-messaging") is string rmq && !String.IsNullOrEmpty(rmq))
            {
                cfg.UseRabbitMq(rmq, "rebus-webapp2");
            }
        });

        builder.Services.AddRebus(cfg => cfg.Transport(configureTransport));

        builder.Services.AddRebusHandler<EchoRequestHandler>();

        return builder;
    }
}
