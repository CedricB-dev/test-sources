using BlazorTDD.App.Models;

namespace BlazorTDD.App.Services;

public interface IWeatherForecastService
{
    Task<WeatherForecast[]> Get();
}

public class WeatherForecastService : IWeatherForecastService
{
    private readonly string[] _summaries = { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
    
    public async Task<WeatherForecast[]> Get()
    {
        await Task.Delay(2000);

        var startDate = DateOnly.FromDateTime(DateTime.Now);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)]
        }).ToArray();
    }
}