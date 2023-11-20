using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BlazorCommunityTools.App.Components.Pages
{
    public partial class CounterViewModel : ViewModelBase
    {
        [ObservableProperty]
        private int _currentCount = 0;

        [RelayCommand]
        private void IncrementCount() => CurrentCount++;
    }
}
