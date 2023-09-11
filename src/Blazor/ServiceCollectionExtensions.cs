using Microsoft.Extensions.DependencyInjection.Extensions;
using Passwordless.Blazor;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPasswordlessBlazor(this IServiceCollection services, Action<PasswordlessClientOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.TryAddScoped<PasswordlessInterop>();

        return services;
    }
}
