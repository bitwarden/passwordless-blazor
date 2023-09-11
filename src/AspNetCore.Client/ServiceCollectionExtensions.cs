using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Passwordless.AspNetCore.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // TODO: Add overload with customized register body
    public static IServiceCollection AddPasswordlessAspNetCoreClient(this IServiceCollection services, Action<PasswordlessAspNetCoreClientOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient<PasswordlessAspNetCoreClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<PasswordlessAspNetCoreClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BackendUrl);
        });

        services.TryAddScoped<PasswordlessAspNetCoreClient<PasswordlessRegisterRequest>>(
            sp => sp.GetRequiredService<PasswordlessAspNetCoreClient>()
        );

        return services;
    }
}
