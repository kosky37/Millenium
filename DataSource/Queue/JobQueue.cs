using System.Threading.Channels;

namespace DataSource.Queue;

public interface IJobQueue
{
    void Enqueue(JobItem job);
    Task<JobItem> DequeueAsync(CancellationToken cancellationToken);
}

public class InMemoryJobQueue : IJobQueue
{
    private readonly Channel<JobItem> _queue = Channel.CreateUnbounded<JobItem>();

    public void Enqueue(JobItem job)
    {
        _queue.Writer.TryWrite(job);
    }

    public async Task<JobItem> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}