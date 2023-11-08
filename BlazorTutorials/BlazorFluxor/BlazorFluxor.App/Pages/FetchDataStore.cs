using BlazorFluxor.App.Data;
using Fluxor;

namespace BlazorFluxor.App.Pages;

[FeatureState]
public record WeatherState
{
    public bool IsLoading { get; init; }
    public IReadOnlyList<WeatherForecast> Forecasts { get; init; } = new List<WeatherForecast>();
}

public class FetchDataAction
{
}

public class FetchDataResultAction
{
    public FetchDataResultAction(IReadOnlyList<WeatherForecast> forecasts)
    {
        Forecasts = forecasts;
    }

    public IReadOnlyList<WeatherForecast> Forecasts { get; init; }
}

public static class WeatherReducers
{
    [ReducerMethod(typeof(FetchDataAction))]
    public static WeatherState ReduceFetchDataAction(WeatherState _) =>
        new() { IsLoading = true, Forecasts = Array.Empty<WeatherForecast>() };

    // [ReducerMethod]
    // public static WeatherState ReduceFetchDataResultAction(
    //     WeatherState state, FetchDataResultAction action) =>
    //     new() { IsLoading = false, Forecasts = action.Forecasts };
}

public class FetchDataResultReducers : Reducer<WeatherState, FetchDataResultAction>
{
    public override WeatherState Reduce(WeatherState state, FetchDataResultAction action)
    {
        return new() { IsLoading = false, Forecasts = action.Forecasts };
    }
}

public class Effects
{
    private readonly WeatherForecastService _weatherForecastService;

    public Effects(WeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [EffectMethod(typeof(FetchDataAction))]
    public async Task HandleFetchDataAction(IDispatcher dispatcher)
    {
        var forecasts = await _weatherForecastService.GetForecastAsync(DateOnly.FromDateTime(DateTime.Now));
        dispatcher.Dispatch(new FetchDataResultAction(forecasts));
    }
}