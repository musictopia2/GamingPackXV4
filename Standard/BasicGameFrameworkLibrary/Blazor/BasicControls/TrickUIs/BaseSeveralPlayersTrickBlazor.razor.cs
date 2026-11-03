namespace BasicGameFrameworkLibrary.Blazor.BasicControls.TrickUIs;
public partial class BaseSeveralPlayersTrickBlazor<P, B, S, T> : IHandleAsync<AnimateTrickEventModel>, IDisposable
    where P : IPlayerTrick<S, T>, new()
    where B : BasicTrickAreaObservable<S, T>, IMultiplayerTrick<S, T, P>
    where S : IFastEnumSimple
    where T : class, ITrickCard<S>, new()
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public RenderFragment<T>? ChildContent { get; set; } //this is the card that will actually appear.
    [Parameter]
    public B? DataContext { get; set; }
    [Parameter]
    public bool ExtraLongSecondColumn { get; set; } = false;
    private readonly AnimateTrickClass<S, T> _animates;
#pragma warning disable IDE0052 // Remove unread private members
    private IEventAggregator? Aggregator { get; set; }
#pragma warning restore IDE0052 // Remove unread private members
    private SizeF _tempSize;
    private bool _disposedValue;
    public BaseSeveralPlayersTrickBlazor()
    {
        _animates = new ();
        _animates.LongestTravelTime = 200;
        _animates.StateChanged = () => InvokeAsync(StateHasChanged);
        _animates.GetStartingPoint = GetStartingPoint;
        T card = new();
        _tempSize = card.DefaultSize;
    }
    private PointF GetStartingPoint(int index)
    {
        TrickCoordinate trick = DataContext!.ViewList![index];
        int x;
        if (trick.Column == 1)
        {
            x = 0;
        }
        else
        {
            x = GetLeft(trick.Column);
        }
        int y = GetCardTop(trick.Row);
        return new PointF(x, y);
    }
    async Task IHandleAsync<AnimateTrickEventModel>.HandleAsync(AnimateTrickEventModel message)
    {
        if (DataContext == null)
        {
            return;
        }
        await _animates.MovePiecesAsync(DataContext);
    }
    protected override void OnInitialized()
    {
        Aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
        base.OnInitialized();
    }
    private partial void Subscribe();
    private partial void Unsubscribe(); 
    private int GetCardIndex(T card) => DataContext!.CardList.IndexOf(card);


    private DeckRegularDict<T>? _subscribedCards;

    private void TrickCardsChanged()
    {
        _ = InvokeAsync(() =>
        {
            if (DataContext?.ViewList is not null)
            {
                foreach (var card in DataContext.CardList)
                {
                    card.Location = GetStartingPoint(GetCardIndex(card));
                }
            }

            StateHasChanged();
        });
    }
    protected override void OnParametersSet()
    {
        var cards = DataContext?.CardList;

        if (ReferenceEquals(_subscribedCards, cards) == false)
        {
            _subscribedCards?.StateChanged -= TrickCardsChanged;

            _subscribedCards = cards;

            _subscribedCards?.StateChanged += TrickCardsChanged;
        }

        if (DataContext?.ViewList is not null)
        {
            foreach (var card in DataContext.CardList)
            {
                card.Location = GetStartingPoint(GetCardIndex(card));
            }
        }

        base.OnParametersSet();
    }
    private readonly int _marginLeft = 10;
    private readonly int _labelHeight = 4;
    private readonly int _marginBottom = 2;
    private int TotalRows => DataContext!.ViewList!.Max(x => x.Row);
    private int TotalColumns => DataContext!.ViewList!.Max(x => x.Column);
    private string GetEntireCss
    {
        get
        {
            // Reserve the same device-scaled pixels used by the card positions.
            int heights = GetCardTop(TotalRows) + CalculateHeight() + _marginBottom.HeightPixels;
            if (TotalColumns == 2)
            {
                float widths;
                if (ExtraLongSecondColumn == false)
                {
                    float firsts = CalculateWidth();
                    widths = firsts + _marginLeft + _marginLeft + 3;
                }
                else
                {
                    widths = 70;
                }
                return $"height: {heights}px; width: {widths}vw;";
            }
            return $"height: {heights}px; width: 50vw;";
        }
    }
    private static string GetCardCss(T card)
    {
        return $"position: absolute; top: {card.Location.Y}px; left: {card.Location.X}vw;";
    }
    private T GetCardInfo(TrickCoordinate trick)
    {
        int index = DataContext!.ViewList!.IndexOf(trick);
        return DataContext.CardList[index];
    }
    private int GetLeft(int column)
    {
        int lefts = _marginLeft + CalculateWidth();
        lefts *= (column - 1);
        return lefts;
    }
    private string GetLeft(TrickCoordinate trick)
    {
        if (trick.Column == 1)
        {
            return "Left: 0px;";
        }
        int lefts = GetLeft(trick.Column);
        return $"left: {lefts}vw;";
    }
    private int RowHeight => _labelHeight.HeightPixels + CalculateHeight() + 2 * _marginBottom.HeightPixels;
    private int GetCardTop(int row)
    {
        return (row - 1) * RowHeight + _labelHeight.HeightPixels + _marginBottom.HeightPixels;
    }
    private string GetLabelTop(TrickCoordinate trick)
    {
        return $"top: {(trick.Row - 1) * RowHeight}px;";
    }
    private string RealLabelHeight => $"font-size: {_labelHeight.HeightPixels}px;";
    private string GetLabelCss(TrickCoordinate trick)
    {
        string lefts = GetLeft(trick);

        string output = $"position: absolute; top: 0px; {lefts} {GetLabelTop(trick)} {RealLabelHeight}";
        return output;
    }
    private int CalculateWidth()
    {
        var firsts = TargetHeight * _tempSize.Width;
        return (int)firsts / (int)_tempSize.Height;
    }
    private int CalculateHeight()
    {
        return TargetHeight.HeightPixels;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Unsubscribe();
                _subscribedCards?.StateChanged -= TrickCardsChanged;
                _subscribedCards = null;
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