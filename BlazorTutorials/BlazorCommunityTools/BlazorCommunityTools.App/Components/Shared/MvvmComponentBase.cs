using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace BlazorCommunityTools.App.Components.Shared;

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