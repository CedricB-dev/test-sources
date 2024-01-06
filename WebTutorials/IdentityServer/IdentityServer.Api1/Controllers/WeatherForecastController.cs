// using IdentityModel.Client;

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api1.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize("ApiReader")]
public class WeatherForecastController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<WeatherForecastController> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
        _logger.LogInformation("{Method} token : {Token}", nameof(Get), token);
        // _httpClient.SetBearerToken(token);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var weatherForecasts = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7154/weatherforecast");
        return weatherForecasts!;
    }
}