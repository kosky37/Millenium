namespace DataSource.Queue;

public class JobItem
{
    public JobItem(string clientId)
    {
        ClientId = clientId;
    }
    
    public Guid Id { get; } = Guid.NewGuid();
    public string ClientId { get; }
    public string? Result { get; private set; }
    public JobStatus Status { get; private set; } = JobStatus.InProgress;
    public string? Error { get; private set; }

    public void MarkAsCompleted(string result)
    {
        Result = result;
        Status = JobStatus.Completed;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
        Status = JobStatus.Failed;
    }
}