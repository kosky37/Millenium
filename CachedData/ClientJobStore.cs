using System.Collections.Concurrent;

namespace CachedData;

public interface IClientJobStore
{
    void Add(string clientId, Guid jobId);
    void Remove(string clientId);
    Guid? GetJobId(string clientId);
}

public class InMemoryClientJobStore : IClientJobStore
{
    private readonly ConcurrentDictionary<string, Guid> _store = new();

    public void Add(string clientId, Guid jobId)
    {
        _store[clientId] = jobId;
    }
    
    public void Remove(string clientId)
    {
        _store.TryRemove(clientId, out _);
    }
    
    public Guid? GetJobId(string clientId)
    {
        _store.TryGetValue(clientId, out var jobId);
        return jobId;
    }
}