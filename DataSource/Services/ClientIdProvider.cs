namespace DataSource.Services;

public interface IClientIdProvider
{
    string GetClientId();
}

public class ClientIdProvider : IClientIdProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientIdProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetClientId()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context == null)
            throw new InvalidOperationException("No HTTP context available.");
        
        if(context.Request.Cookies.TryGetValue("X-ClientId", out var clientId))
        {
            return clientId;
        }
        
        throw new InvalidOperationException("No clientId provided"); // handle this exception
    }
}