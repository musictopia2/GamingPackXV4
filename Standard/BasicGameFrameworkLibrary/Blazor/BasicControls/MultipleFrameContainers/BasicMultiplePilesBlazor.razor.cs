namespace BasicGameFrameworkLibrary.Blazor.BasicControls.MultipleFrameContainers;
[CustomTag("", AlsoNoTags = true)]
public partial class BasicMultiplePilesBlazor<D> : IDisposable, IHandleAsync<AnimateCardInPileEventModel<D>>, IHandle<ResetCardsEventModel>
    where D : class, IDeckObject, new()
{
    private enum EnumLocation
    {
        None, Top, Bottom
    }
    private AnimateDeckImageTimer? _animates;
    private bool _disposedValue;
    private void ShowChange()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    private IEventAggregator? Aggregator { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<D>? Piles { get; set; }
    [Parameter]
    public string AnimationTag { get; set; } = ""; //you must have one.  otherwise can't show either.
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public bool Inline { get; set; } = true;
    protected override void OnInitialized()
    {
        _animates = new();
        D item = new();
        Aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe(AnimationTag);
        _animates.StateChanged = ShowChange;
        _animates.LongestTravelTime = 200;
        base.OnInitialized();
    }
    private partial void Subscribe(string tag);
    private partial void Unsubscribe(string tag);
    protected override void OnParametersSet()
    {
        TopLocation = TargetHeight * -1;
        BottomLocation = TargetHeight;
        base.OnParametersSet();
    }
    [Parameter]
    public RenderFragment<D>? CanvasTemplate { get; set; }
    [Parameter]
    public RenderFragment<BasicPileInfo<D>>? MainTemplate { get; set; }
    [Parameter]
    public RenderFragment? MiscRowTemplate { get; set; }
    [Parameter]
    public int RowPlaceAfter { get; set; }
    private int GetRowCount
    {
        get
        {
            if (MiscRowTemplate == null || RowPlaceAfter == 0)
            {
                return Piles!.Rows;
            }
            return Piles!.Rows + 1;
        }
    }
    private double ObjectLocation { get; set; } = 0;
    private double TopLocation { get; set; }
    private double BottomLocation { get; set; } //if its 8, then has to figure out what else to do (?)
    private BasicPileInfo<D>? GetPile(int column, int row)
    {
        return Piles!.PileList!.SingleOrDefault(x => x.Column == column && x.Row == row); //maybe we have it and maybe we don't
    }
    internal BasicPileInfo<D>? AnimatePile { get; set; }
    private D? CurrentObject { get; set; }
    internal bool ShowPrevious { get; set; }
    internal D? RenderCard { get; set; }
    internal bool CanRender()
    {
        return _animates!.CurrentYLocation == -1000 || ShowPrevious == true;
    }
    private D? AltShowImage { get; set; }
    private D? AnimateDeckImage { get; set; }
    private void PopulateCardToShow()
    {
        if (AltShowImage != null)
        {
            RenderCard = AltShowImage;
        }
        else if (AnimateDeckImage != null)
        {
            RenderCard = AnimateDeckImage;
        }
        else
        {
            RenderCard = null;
        }
    }
    async Task IHandleAsync<AnimateCardInPileEventModel<D>>.HandleAsync(AnimateCardInPileEventModel<D> message)
    {
        ShowPrevious = false;
        AnimatePile = message.ThisPile;
        AltShowImage = null;
        AnimateDeckImage = message.ThisCard;
        CurrentObject = message.ThisCard!;
        CurrentObject.IsSelected = false;
        switch (message.Direction)
        {
            case EnumAnimcationDirection.StartUpToCard:
                _animates!.LocationYFrom = TopLocation;
                _animates.LocationYTo = ObjectLocation;

                break;
            case EnumAnimcationDirection.StartDownToCard:
                _animates!.LocationYFrom = BottomLocation;
                _animates.LocationYTo = ObjectLocation + 1.7;
                break;
            case EnumAnimcationDirection.StartCardToUp:
                ShowPrevious = true;
                AltShowImage = Piles!.GetLastCard(message.ThisPile!);
                _animates!.LocationYFrom = ObjectLocation;
                _animates.LocationYTo = TopLocation;
                break;
            case EnumAnimcationDirection.StartCardToDown:
                ShowPrevious = true;
                AltShowImage = Piles!.GetLastCard(message.ThisPile!);
                _animates!.LocationYFrom = ObjectLocation;
                _animates.LocationYTo = BottomLocation;
                break;
            default:
                break;
        }
        await _animates!.DoAnimateAsync();
    }
    void IHandle<ResetCardsEventModel>.Handle(ResetCardsEventModel message)
    {
        AltShowImage = null;
        AnimateDeckImage = null;
        AnimatePile = null;
    }
    private string GetTopText => $"Top: {_animates!.CurrentYLocation}vh; Left: 2px;";
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Unsubscribe(AnimationTag);
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}