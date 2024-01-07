using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;

namespace OpenIddict.Web2.Services;

public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccessTokenManagementService _userAccessTokenManagementService;

    public WeatherService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IUserAccessTokenManagementService userAccessTokenManagementService)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _userAccessTokenManagementService = userAccessTokenManagementService;
    }

    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        var user = _httpContextAccessor.HttpContext.User;
        var token = await _userAccessTokenManagementService.GetUserAccessTokenAsync(user);
        var client = _httpClientFactory.CreateClient("weather");
        client.SetBearerToken(token);
        var response = await client.GetFromJsonAsync<WeatherForecast[]>("weatherforecast");
        return response;
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}