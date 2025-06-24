using CachedData.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace CachedData.Services;

public interface ICachedJobService
{
    Task<DataResponse> GetOrCreateJobResultAsync(string clientId, CancellationToken cancellationToken)
}

public class CachedJobService : ICachedJobService
{
    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IClientJobStore _clientJobStore;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public CachedJobService(
        IMemoryCache cache,
        IHttpClientFactory httpClientFactory,
        IClientJobStore clientJobStore)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _clientJobStore = clientJobStore;
    }

    public async Task<DataResponse> GetOrCreateJobResultAsync(string clientId, CancellationToken cancellationToken) // use separate model(DataResponse)
    {
        var cacheKey = $"job-result:{clientId}";

        if (_cache.TryGetValue<string>(cacheKey, out var cachedResult))
        {
            return new DataResponse()
            {
                Data = cachedResult,
                DataReady = true
            };
        }
        
        var client = _httpClientFactory.CreateClient("DataSource");

        var jobId = _clientJobStore.GetJobId(clientId);

        if (jobId is null)
        {
            var scheduleJobResponse = await client.GetAsync($"api/data?clientId={clientId}", cancellationToken); //change in the other service to query param and use the cookie clientId provider in this service instead
            
            if (!scheduleJobResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed with status {scheduleJobResponse.StatusCode}");
            }
            
            var jobIdString = await scheduleJobResponse.Content.ReadAsStringAsync(cancellationToken);
            jobId = Guid.Parse(jobIdString.Trim('"'));
            
            _clientJobStore.Add(clientId, jobId.Value);
            return new DataResponse
            {
                Data = null,
                DataReady = false
            };
        }
        
        await client.GetAsync($"api/data/job{jobId}/status?clientId={clientId}", cancellationToken);

        var jobStatusResponse = await client.GetAsync($"api/data/job{jobId}/status?clientId={clientId}", cancellationToken);
        
        if (!jobStatusResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Failed with status {jobStatusResponse.StatusCode}");
        }
        
        //parse the status

        if (jobStatus == JobStatus.Success)
        {
            var jobResultResponse = await client.GetAsync($"api/data/job{jobId}/status?clientId={clientId}", cancellationToken);
        
            if (!jobResultResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed with status {jobStatusResponse.StatusCode}");
            }
        
            var data = await jobResultResponse.Content.ReadAsStringAsync(cancellationToken);

            _clientJobStore.Remove(clientId);
            _cache.Set(cacheKey, data, CacheDuration);

            return new DataResponse
            {
                Data = data,
                DataReady = true
            };
        }

        return new DataResponse
        {
            Data = null,
            DataReady = false
        };
    }
}