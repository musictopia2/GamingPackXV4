namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.SingleMiscPiles;
[CustomTag("", AlsoNoTags = true)]
public partial class BaseSingleMiscPileBlazor<D> : IDisposable, IHandleAsync<AnimateCardInPileEventModel<D>>, IHandle<ResetCardsEventModel>
    where D : class, IDeckObject, new()
{
    private enum EnumLocation
    {
        None, Top, Bottom
    }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public SingleObservablePile<D>? SinglePile { get; set; }
    [Parameter]
    public bool UseKey { get; set; } = true; //allow the possibility of setting to false to see if that helps for a game like payday.
    private D GetCard
    {
        get
        {
            if (AltShowImage is not null)
            {
                return AltShowImage;
            }
            if (AnimateDeckImage is not null)
            {
                return AnimateDeckImage;
            }
            return SinglePile!.CurrentCard;
        }
    }
    [Parameter]
    public string PileAnimationTag { get; set; } = "maindiscard";
    private AnimateDeckImageTimer? _animates;
    private void ShowChange()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    [Parameter]
    public RenderFragment<D>? CanvasTemplate { get; set; } //can't use generics because that control is responsible for knowing which one it is (via event aggregation).
    [Parameter]
    public RenderFragment<D>? MainTemplate { get; set; }
    private IEventAggregator? Aggregator { get; set; }
    protected override void OnInitialized()
    {
        _animates = new();
        Aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe(PileAnimationTag);
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
    private bool _showPrevious;
    private bool _disposedValue;

    private D? AltShowImage { get; set; }
    private D? AnimateDeckImage { get; set; }
    private double ObjectLocation { get; set; } = 0;
    private double TopLocation { get; set; }
    private double BottomLocation { get; set; }
    async Task IHandleAsync<AnimateCardInPileEventModel<D>>.HandleAsync(AnimateCardInPileEventModel<D> message)
    {
        _showPrevious = false;
        AltShowImage = null;
        AnimateDeckImage = message.ThisCard;
        AnimateDeckImage!.IsSelected = false;
        switch (message.Direction)
        {
            case EnumAnimcationDirection.StartUpToCard:
                _animates!.LocationYFrom = TopLocation;
                _animates.LocationYTo = ObjectLocation;
                break;
            case EnumAnimcationDirection.StartDownToCard:
                _animates!.LocationYFrom = BottomLocation;
                _animates.LocationYTo = ObjectLocation;

                break;
            case EnumAnimcationDirection.StartCardToUp:
                _showPrevious = true;
                AltShowImage = SinglePile!.CurrentCard;
                _animates!.LocationYFrom = ObjectLocation;
                _animates.LocationYTo = TopLocation;
                break;
            case EnumAnimcationDirection.StartCardToDown:
                _showPrevious = true;
                AltShowImage = SinglePile!.CurrentCard;
                _animates!.LocationYFrom = ObjectLocation;
                _animates.LocationYTo = BottomLocation;
                break;
            default:
                break;
        }
        await _animates!.DoAnimateAsync();
    }
    private void Reset()
    {
        AltShowImage = null;
        AnimateDeckImage = null;
    }
    void IHandle<ResetCardsEventModel>.Handle(ResetCardsEventModel message)
    {
        Reset();
    }
    private string GetTopText => $"Top: {_animates!.CurrentYLocation}vh;";
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Unsubscribe(PileAnimationTag);
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}