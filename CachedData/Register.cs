
namespace CachedData;

public static class Register
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IClientJobStore, InMemoryClientJobStore>();
    }
}