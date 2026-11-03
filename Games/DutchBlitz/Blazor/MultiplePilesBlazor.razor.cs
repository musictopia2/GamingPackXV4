namespace DutchBlitz.Blazor;
public partial class MultiplePilesBlazor : IHandleAsync<DutchBlitzCardsChangedEventModel>, IDisposable
{
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<DutchBlitzCardInformation>? Piles { get; set; }
    [Parameter]
    public string AnimationTag { get; set; } = "";
    [Parameter]
    public bool Inline { get; set; } = true;
    private partial void Subscribe();
    private partial void Unsubscribe();
    private IEventAggregator? _aggregator;
    private int _renderVersion;
    protected override void OnInitialized()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
        base.OnInitialized();
    }
    async Task IHandleAsync<DutchBlitzCardsChangedEventModel>.HandleAsync(DutchBlitzCardsChangedEventModel message)
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