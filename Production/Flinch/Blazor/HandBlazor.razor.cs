namespace Flinch.Blazor;
public partial class HandBlazor : IHandleAsync<FlinchChangedEventModel>, IDisposable
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
    async Task IHandleAsync<FlinchChangedEventModel>.HandleAsync(FlinchChangedEventModel message)
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
