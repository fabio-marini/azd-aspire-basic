using Aspire.Hosting.Azure;
using Microsoft.Extensions.Configuration;

public static class Extensions
{
    public static IResourceBuilder<IResourceWithConnectionString>? AddFromConfiguration(this IDistributedApplicationBuilder builder, string name, string key)
    {
        var res = builder.Resources.OfType<IResourceWithConnectionString>().FirstOrDefault(res => res.Name == name);

        if (res != null)
        {
            // resource already added to builder, return it
            return builder.CreateResourceBuilder<IResourceWithConnectionString>(res);
        }

        var uri = builder.Configuration.GetValue<string>(key);

        if (uri is string && !String.IsNullOrEmpty(uri))
        {
            // config contains a connection string, create resource from that
            return builder.AddConnectionString(name, bld => bld.AppendLiteral(uri));
        }

        return default;
    }

    public static IResourceBuilder<AzureStorageResource> GetStorageResource(this IDistributedApplicationBuilder builder, string name)
    {
        // if storage resource already exists, use it - otherwise create a new one
        return builder.Resources.OfType<AzureStorageResource>().Any(res => res.Name == name)
            ? builder.CreateResourceBuilder<AzureStorageResource>(name)
            : builder.AddAzureStorage(name)
            ;
    }
}
