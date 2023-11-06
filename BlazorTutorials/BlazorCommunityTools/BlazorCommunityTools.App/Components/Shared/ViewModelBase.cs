using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BlazorCommunityTools.App.Components.Shared;

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