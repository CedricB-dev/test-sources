using Fluxor;

namespace BlazorFluxor.App.Pages;

//[FeatureState]
public record CounterState
{
    public int CurrentCount { get; init; }
}

public class CounterFeature : Feature<CounterState>
{
    public override string GetName() => "Counter";

    protected override CounterState GetInitialState()
    {
        return new CounterState
        {
            CurrentCount = 0
        };
    }
}

public class IncrementCounterAction { }

public static class CounterReducers
{
    [ReducerMethod(typeof(IncrementCounterAction))]
    public static CounterState ReduceIncrementCounterAction(CounterState state) =>
        new() { CurrentCount = state.CurrentCount + 1 };
}