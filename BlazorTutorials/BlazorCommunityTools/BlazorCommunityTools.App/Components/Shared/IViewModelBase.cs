using System.ComponentModel;

namespace BlazorCommunityTools.App.Components.Shared;

public interface IViewModelBase : INotifyPropertyChanged
{
    Task OnInitializeAsync();
    Task Loaded();
}