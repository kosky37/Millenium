using CachedData.Responses;
using CachedData.Services;
using Microsoft.AspNetCore.Mvc;

namespace CachedData.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly ICachedJobService _cachedJobService;

    public DataController(ICachedJobService cachedJobService)
    {
        _cachedJobService = cachedJobService;
    }

    [HttpGet]
    public DataResponse Get(CancellationToken cancellationToken)
    {
        return _cachedJobService.GetOrCreateJobResultAsync(clientId, cancellationToken);// get clientId from cookie
    }   
}