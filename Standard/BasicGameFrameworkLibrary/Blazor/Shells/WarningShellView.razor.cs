namespace BasicGameFrameworkLibrary.Blazor.Shells;
public partial class WarningShellView : IHandleAsync<WarningEventModel>, IDisposable
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    protected bool Opened;
    private WarningEventModel? _warning;
    private IEventAggregator? _aggregator;
    protected override void OnInitialized()
    {
        _aggregator = aa1.Resolver!.Resolve<IEventAggregator>();
        Subscribe();
        base.OnInitialized();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    Task IHandleAsync<WarningEventModel>.HandleAsync(WarningEventModel message)
    {
        _warning = message;
        Opened = true;
        InvokeAsync(StateHasChanged);
        return Task.CompletedTask;
    }
    private async Task ConfirmAsync(bool rets)
    {
        SelectionChosenEventModel results = new();
        if (rets == false)
        {
            results.OptionChosen = EnumOptionChosen.No;
        }
        else
        {
            results.OptionChosen = EnumOptionChosen.Yes;
        }
        Opened = false;
        await _aggregator!.PublishAsync(results);
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        Unsubscribe();
    }
}