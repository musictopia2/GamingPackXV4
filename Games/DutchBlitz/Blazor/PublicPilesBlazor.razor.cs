namespace DutchBlitz.Blazor;
public partial class PublicPilesBlazor : IHandleAsync<DutchBlitzCardsChangedEventModel>, IDisposable
{
    [CascadingParameter]
    public DutchBlitzVMData? GameData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private string RealHeight => $"{TargetHeight.HeightPixels}px";
    private enum EnumLocation
    {
        None, Top, Bottom
    }
    private async Task NewClickedAsync()
    {
        await GameData!.PublicPiles1.NewCommand!.ExecuteAsync(null);
    }

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
        if (ReferenceEquals(message.Target, GameData!.PublicPiles1))
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