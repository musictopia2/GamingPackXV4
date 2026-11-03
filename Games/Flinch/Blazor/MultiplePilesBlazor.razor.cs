namespace Flinch.Blazor;
public partial class MultiplePilesBlazor : IHandleAsync<FlinchChangedEventModel>, IDisposable
{
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<FlinchCardInformation>? Piles { get; set; }
    [Parameter]
    public string AnimationTag { get; set; } = "";
    [Parameter]
    public bool Inline { get; set; } = true;
    private int _renderVersion;
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
        if (ReferenceEquals(message.Target, Piles))
        {
            await InvokeAsync(() =>
            {
                _renderVersion++;
                StateHasChanged();
            });
        }
    }
    public void Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
}