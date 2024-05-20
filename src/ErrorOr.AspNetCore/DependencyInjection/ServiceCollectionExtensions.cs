using ErrorOr;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddErrorOr(this IServiceCollection services, Action<ErrorOrOptions> options)
    {
        options.Invoke(ErrorOrOptions.Instance);
        services.AddSingleton(ErrorOrOptions.Instance);
        return services;
    }
}
