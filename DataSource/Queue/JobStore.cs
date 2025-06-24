using System.Collections.Concurrent;

namespace DataSource.Queue;

public interface IJobStore
{
    void Add(JobItem job);
    JobItem? Get(Guid jobId);
}

public class InMemoryJobStore : IJobStore
{
    private readonly ConcurrentDictionary<Guid, JobItem> _store = new();

    public void Add(JobItem job)
    {
        _store[job.Id] = job;
    }

    public JobItem? Get(Guid jobId)
    {
        _store.TryGetValue(jobId, out var job);
        return job;
    }
}