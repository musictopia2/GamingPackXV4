namespace Skipbo.Blazor;
public partial class HandBlazor : IHandleAsync<SkipboCardsChangedEventModel>, IDisposable
{
    private IEventAggregator? _aggregator;
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected override void OnInitialized()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
        base.OnInitialized();
    }
    async Task IHandleAsync<SkipboCardsChangedEventModel>.HandleAsync(SkipboCardsChangedEventModel message)
    {
        if (ReferenceEquals(message.Target, Hand))
        {
            await InvokeAsync(StateHasChanged);
        }
    }
    public void Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
}
