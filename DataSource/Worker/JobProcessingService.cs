using DataSource.Queue;

namespace DataSource.Worker;

public class JobProcessingService : BackgroundService
{
    private readonly ILogger<JobProcessingService> _logger;
    private readonly IJobQueue _jobQueue;

    public JobProcessingService(ILogger<JobProcessingService> logger, IJobQueue jobQueue)
    {
        _logger = logger;
        _jobQueue = jobQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Job Processing Service started.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var job = await _jobQueue.DequeueAsync(cancellationToken);
            
            try
            {
                _logger.LogInformation("Processing job {JobId}", job.Id);
                
                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
                
                job.MarkAsCompleted($"Processed result for client {job.ClientId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job {JobId}", job.Id);
                job.MarkAsFailed(ex.Message);
            }
        }
    }
}
