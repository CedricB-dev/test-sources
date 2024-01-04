
namespace OpenIddict.Web1.Services;

public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        var client = _httpClientFactory.CreateClient("weather");
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