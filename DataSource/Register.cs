using DataSource.Queue;
using DataSource.Services;
using DataSource.Worker;

namespace DataSource;

public static class Register
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IClientIdProvider, ClientIdProvider>();
        services.AddTransient<IRequestCounter, RequestCounter>();
        
        services.AddSingleton<IJobQueue, InMemoryJobQueue>();
        services.AddSingleton<IJobStore, InMemoryJobStore>();
        services.AddTransient<IJobService, JobService>();
        services.AddHostedService<JobProcessingService>();
    }
}