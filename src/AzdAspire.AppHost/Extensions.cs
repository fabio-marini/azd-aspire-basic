using Microsoft.Extensions.Configuration;

/// <summary>
/// Provides extension methods for distributed application resource management.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds a resource with a connection string from the configuration to the distributed application builder.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="key">The configuration key for the connection string.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{IResourceWithConnectionString}"/> for the resource if found or created; otherwise, <c>null</c>.
    /// </returns>
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
}
