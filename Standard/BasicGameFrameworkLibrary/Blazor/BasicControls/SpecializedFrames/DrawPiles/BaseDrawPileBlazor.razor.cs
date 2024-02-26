namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.DrawPiles;
[CustomTag("")]
public partial class BaseDrawPileBlazor<D> : IDisposable, IHandleAsync<AnimateCardInPileEventModel<D>>
     where D : class, IDeckObject, new()
{
    private enum EnumLocation
    {
        None, Top, Bottom
    }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15; //can experiment to see what the default should be.
    [Parameter]
    public DeckObservablePile<D>? DeckPile { get; set; }
    [Parameter]
    public string DeckAnimationTag { get; set; } = "maindeck";
    private AnimateDeckImageTimer? _animates;
    public BaseDrawPileBlazor(){ }
    private partial void Subscribe(string tag);
    private partial void Unsubscribe(string tag);
    private void ShowChange()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    [Parameter]
    public RenderFragment<D>? CanvasTemplate { get; set; }
    [Parameter]
    public RenderFragment<D>? MainTemplate { get; set; }
#pragma warning disable IDE0052 // Remove unread private members
    private IEventAggregator? Aggregator { get; set; }
#pragma warning restore IDE0052 // Remove unread private members
    private float GetHalfTop()
    {
        var firsts = _defaultSize.Width * TargetHeight;
        return firsts / _defaultSize.Height;
    }
    private string GetMarginsForTextCenter()
    {
        float tops = GetHalfTop();
        return $"margin-top: -{tops}vh";
    }
    private SizeF _defaultSize;
    private bool _disposedValue;
    protected override void OnInitialized()
    {
        _animates = new();
        D card = new();
        _defaultSize = card.DefaultSize;
        Aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe(DeckAnimationTag);
        _animates.StateChanged = ShowChange;
        _animates.LongestTravelTime = 200;
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        TopLocation = TargetHeight * -1;
        BottomLocation = TargetHeight;
        base.OnParametersSet();
    }
    private D? AnimateDeckImage { get; set; }
    private double ObjectLocation { get; set; } = 0;
    private double TopLocation { get; set; }
    private double BottomLocation { get; set; }
    async Task IHandleAsync<AnimateCardInPileEventModel<D>>.HandleAsync(AnimateCardInPileEventModel<D> message)
    {
        var card = new D();
        card.Populate(message.ThisCard!.Deck);
        if (DeckPile!.DeckStyle == EnumDeckPileStyle.AlwaysKnown)
        {
            card.IsUnknown = false;
        }
        else
        {
            card.IsUnknown = true;
        }
        AnimateDeckImage = card;
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
                _animates!.LocationYFrom = ObjectLocation;
                _animates.LocationYTo = TopLocation;
                break;
            case EnumAnimcationDirection.StartCardToDown:
                _animates!.LocationYFrom = ObjectLocation;
                _animates.LocationYTo = BottomLocation;
                break;
            default:
                break;
        }
        await _animates!.DoAnimateAsync();
    }
    private string GetTopText => $"Top: {_animates!.CurrentYLocation}vh;";
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Unsubscribe(DeckAnimationTag);
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