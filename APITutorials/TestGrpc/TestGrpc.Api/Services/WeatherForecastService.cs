using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace TestGrpc.Api.Services;

public class WeatherForecastService : Grpc.WeatherForecastService.WeatherForecastServiceBase
{
    public override Task<Grpc.WeatherForecastResponse> GetWeatherForecast(Empty request, ServerCallContext context)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        var forecast = Enumerable.Range(1, 5).Select(index =>
        {
            var date = DateTime.UtcNow.AddDays(index);
            var temperatureC = Random.Shared.Next(-20, 55);
            var weatherForecast = new Grpc.WeatherForecast
            {
                Date = Timestamp.FromDateTime(date),
                TemperatureC = temperatureC,
                TemperatureF = 32 + (int)(temperatureC / 0.5556),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            };
            return weatherForecast;
        }).ToArray();
            
        return Task.FromResult(new Grpc.WeatherForecastResponse { Forecasts = { forecast }});
    }
}