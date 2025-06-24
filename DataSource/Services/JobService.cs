using DataSource.Queue;

namespace DataSource.Services;

public interface IJobService
{
    Guid ScheduleJob(IClientIdProvider clientIdProvider);
    JobItem? GetJob(Guid jobId);
}

public class JobService : IJobService
{
    private readonly IJobQueue _jobQueue;
    private readonly IJobStore _jobStore;

    public JobService(IJobQueue jobQueue, IJobStore jobStore)
    {
        _jobQueue = jobQueue;
        _jobStore = jobStore;
    }
    
    public Guid ScheduleJob(IClientIdProvider clientIdProvider)
    {
        var clientId = clientIdProvider.GetClientId();

        var job = new JobItem(clientId);
        
        _jobQueue.Enqueue(job);
        _jobStore.Add(job);

        return job.Id;
    }

    public JobItem? GetJob(Guid jobId)
    {
        return _jobStore.Get(jobId);
    }
}

