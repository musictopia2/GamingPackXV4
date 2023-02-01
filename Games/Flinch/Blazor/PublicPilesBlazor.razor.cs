namespace Flinch.Blazor;
[CustomTag("", AlsoNoTags = true)]
public partial class PublicPilesBlazor : IDisposable, IHandleAsync<AnimateCardInPileEventModel<FlinchCardInformation>>
{
    [CascadingParameter]
    public FlinchVMData? GameData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private string RealHeight => TargetHeight.HeightString();
    private enum EnumLocation
    {
        None, Top, Bottom
    }
    private readonly AnimateDeckImageTimer _animates; //had to use this one instead of the new animation i tried but failed to get to work properly.
    public PublicPilesBlazor()
    {
        Aggregator = aa1.Resolver!.Resolve<IEventAggregator>();
        _animates = new AnimateDeckImageTimer(); //well see on desktop mode.
    }
    private partial void Subscribe(string tag);
    private partial void Unsubscribe(string tag);
    private void ShowChange()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    private IEventAggregator Aggregator { get; set; }
    [Parameter]
    public string AnimationTag { get; set; } = "";
    protected override void OnInitialized()
    {
        FlinchCardInformation item = new();
        Subscribe(AnimationTag);
        _animates.StateChanged = ShowChange;
        _animates.LongestTravelTime = 200;
        CommandContainer command = aa1.Resolver!.Resolve<CommandContainer>();
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        _topLocation = TargetHeight * -1;
        _bottomLocation = TargetHeight;
        base.OnParametersSet();
    }
    private readonly double _objectLocation = 0;
    private double _topLocation;
    private double _bottomLocation;
    private BasicPileInfo<FlinchCardInformation>? AnimatePile { get; set; } //i think
    private FlinchCardInformation? CurrentObject { get; set; }
    private int _animateIndex;
    async Task IHandleAsync<AnimateCardInPileEventModel<FlinchCardInformation>>.HandleAsync(AnimateCardInPileEventModel<FlinchCardInformation> message)
    {
        AnimatePile = message.ThisPile;
        _animateIndex = GameData!.PublicPiles.PileList.IndexOf(AnimatePile!);
        CurrentObject = message.ThisCard!;
        switch (message.Direction)
        {
            case EnumAnimcationDirection.StartUpToCard:
                _animates.LocationYFrom = _topLocation;
                _animates.LocationYTo = _objectLocation + 1.6;
                break;
            case EnumAnimcationDirection.StartDownToCard:
                _animates.LocationYFrom = _bottomLocation;
                _animates.LocationYTo = _objectLocation;
                break;
            case EnumAnimcationDirection.StartCardToUp:
                _animates.LocationYFrom = _objectLocation;
                _animates.LocationYTo = _topLocation;
                break;
            case EnumAnimcationDirection.StartCardToDown:
                _animates.LocationYFrom = _objectLocation;
                _animates.LocationYTo = _bottomLocation;
                break;
            default:
                break;
        }
        await _animates.DoAnimateAsync();
    }
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    void IDisposable.Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        Unsubscribe(AnimationTag);
    }
    private string GetTopText => $"Top: {_animates.CurrentYLocation}vh;";
}
