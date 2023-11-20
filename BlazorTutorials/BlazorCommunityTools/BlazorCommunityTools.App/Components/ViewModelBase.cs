using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace BlazorCommunityTools.App.Components;

public interface IViewModelBase : INotifyPropertyChanged
{
    Task OnInitializeAsync();
    Task Loaded();
}

public abstract partial class ViewModelBase : ObservableObject, IViewModelBase
{
    public virtual async Task OnInitializeAsync()
    {
        await Loaded().ConfigureAwait(true);
    }

    protected virtual void NotifyStateChanged() => OnPropertyChanged();

    [RelayCommand]
    public virtual async Task Loaded()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }
}


public abstract class MvvmComponentBase<TViewModel> : ComponentBase
    where TViewModel : IViewModelBase
{
    [Inject]
    [NotNull]
    public TViewModel ViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        ViewModel.PropertyChanged += (_, _) => StateHasChanged();
        base.OnInitialized();
    }

    protected override Task OnInitializedAsync()
    {
        return ViewModel.OnInitializeAsync();
    }
}