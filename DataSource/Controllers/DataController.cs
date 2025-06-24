using DataSource.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataSource.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly IClientIdProvider _clientIdProvider;
    private readonly IJobService _jobService;
    private readonly IRequestCounter _requestCounter;

    public DataController(IClientIdProvider clientIdProvider, IJobService jobService, IRequestCounter requestCounter)
    {
        _clientIdProvider = clientIdProvider;
        _jobService = jobService;
        _requestCounter = requestCounter;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _requestCounter.IncrementRequestCount();
        if (_requestCounter.IsTenthRequest())
            return BadRequest("Unlucky request");
        
        var jobId = _jobService.ScheduleJob(_clientIdProvider);
        
        return Accepted($"api/data/status/{jobId}");
    }
    
    [HttpGet("job/{jobId}/status")]
    public IActionResult GetJobStatus(Guid jobId)
    {
        _requestCounter.IncrementRequestCount();
        if (_requestCounter.IsTenthRequest())
            return BadRequest("Unlucky request");
        
        var job = _jobService.GetJob(jobId);
        
        return job is null ? NotFound() : Ok(job.Status);
    }
    
    [HttpGet("job/{jobId}/result")]
    public IActionResult GetJobResult(Guid jobId)
    {
        _requestCounter.IncrementRequestCount();
        if (_requestCounter.IsTenthRequest())
            return BadRequest("Unlucky request");
        
        var job = _jobService.GetJob(jobId);
        
        return job is null ? NotFound() : Ok(job.Result);
    }
}