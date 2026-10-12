namespace Skipbo.Blazor;
public partial class MultiplePilesBlazor : IHandleAsync<SkipboCardsChangedEventModel>, IDisposable
{
    private IEventAggregator? _aggregator;
    private int _renderVersion;
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
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<SkipboCardInformation>? Piles { get; set; }
    [Parameter]
    public string AnimationTag { get; set; } = "";
    [Parameter]
    public bool Inline { get; set; } = true;
    private string RealHeight => TargetHeight.HeightString();
}