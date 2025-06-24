namespace DataSource.Services;

public interface IRequestCounter
{
    void IncrementRequestCount();
    bool IsTenthRequest();
}

public class RequestCounter : IRequestCounter
{
    private int _requestCount;
    
    public void IncrementRequestCount()
    {
        _requestCount++;
    }
    
    public bool IsTenthRequest()
    {
        return _requestCount % 10 == 0;
    }
}