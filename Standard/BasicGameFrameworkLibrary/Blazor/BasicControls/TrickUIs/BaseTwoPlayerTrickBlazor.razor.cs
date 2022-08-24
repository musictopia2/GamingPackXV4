namespace BasicGameFrameworkLibrary.Blazor.BasicControls.TrickUIs;
public partial class BaseTwoPlayerTrickBlazor<S, T> : IHandleAsync<AnimateTrickEventModel>, IDisposable
    where S : IFastEnumSimple
    where T : class, ITrickCard<S>, new()
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15; //its the view height.
    [Parameter]
    public RenderFragment<T>? ChildContent { get; set; } //this is the card that will actually appear.
    [Parameter]
    public BasicTrickAreaObservable<S, T>? DataContext { get; set; }
    private readonly AnimateTrickClass<S, T> _animates;
    private IEventAggregator? Aggregator { get; set; }
    private SizeF _tempSize;
    private bool _disposedValue;
    public BaseTwoPlayerTrickBlazor()
    {
        _animates = new ();
        _animates.LongestTravelTime = 200; //can adjust as needed.
        _animates.StateChanged = () => InvokeAsync(StateHasChanged);
        _animates.GetStartingPoint = GetStartingPoint;
        T card = new();
        _tempSize = card.DefaultSize; //for proportions.
    }
    private readonly int _marginLeft = 10;
    private readonly int _labelHeight = 4;
    private readonly int _marginBottom = 2;
    private int CalculateWidth()
    {
        var firsts = TargetHeight * _tempSize.Width;
        return (int)firsts / (int)_tempSize.Height;
    }
    private int CalculateHeight()
    {
        var firsts = TargetHeight * _tempSize.Width;
        return (int)firsts / (int)_tempSize.Width;
    }
    private int GetIntLefts
    {
        get
        {
            int lefts = _marginLeft + CalculateWidth();
            return lefts;
        }
    }
    protected override void OnParametersSet()
    {
        foreach (var card in DataContext!.CardList)
        {
            card.Location = GetStartingPoint(GetCardIndex(card));
        }
        base.OnParametersSet();
    }
    private PointF GetStartingPoint(int index)
    {
        int tops;
        tops = _labelHeight + _marginBottom;
        if (index == 0)
        {
            return new PointF(0, tops);
        }
        return new PointF(GetIntLefts, tops);
    }
    protected override void OnInitialized()
    {
        Aggregator = aa.Resolver!.Resolve<IEventAggregator>();
        Subscribe();
        base.OnInitialized();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    async Task IHandleAsync<AnimateTrickEventModel>.HandleAsync(AnimateTrickEventModel message)
    {
        if (DataContext == null)
        {
            return;
        }
        await _animates.MovePiecesAsync(DataContext);
    }
    private string GetEntireCss
    {
        get
        {
            float heights = _labelHeight + CalculateHeight() + _marginBottom + _marginBottom + _marginBottom;
            float firsts = CalculateWidth();
            float widths = firsts + _marginLeft + _marginLeft + 3;
            return $"height: {heights}vh; width: {widths}vw;";
        }
    }
    private static string GetCardCss(T card)
    {
        return $"position: absolute; top: {card.Location.Y}vh; left: {card.Location.X}vw;";
    }
    private int GetCardIndex(T card) => DataContext!.CardList.IndexOf(card);
    private string GetLabelCss(T card)
    {
        int index = GetCardIndex(card);
        return $"position: absolute; top: 0px; {GetLeft(index)} {RealLabelHeight}";
    }
    private string GetLabelText(T card)
    {
        int index = GetCardIndex(card);
        if (index == 0)
        {
            return "Yours";
        }
        return "Opponents";
    }
    private string GetLeft(int index)
    {
        if (index == 0)
        {
            return "Left: 0px;";
        }
        int lefts = _marginLeft + CalculateWidth();
        return $"left: {lefts}vw;";
    }
    private string RealLabelHeight => $"font-size: {_labelHeight}vh;"; //can adjust as needed.
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Unsubscribe();
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
