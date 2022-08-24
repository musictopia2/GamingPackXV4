namespace BasicGamingUIBlazorLibrary.Animations;
public partial class AnimationVectorCanvas<S> : IHandleAsync<AnimatePieceEventModel<S>>, IDisposable //this requires generics since the other did and we are cascading to use the stuff from the parent board.
    where S : class, IBasicSpace, new()
{
    private bool _disposedValue;
    [CascadingParameter]
    public GridGameBoard<S>? MainBoard { get; set; }
    [Parameter]
    public RenderFragment<S>? ChildContent { get; set; }
    private IEventAggregator? Aggregator { get; set; }
    public AnimationVectorCanvas()
    {
        _animates = new AnimateGrid();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected override void OnInitialized()
    {
        Aggregator = aa.Resolver!.Resolve<IEventAggregator>();
        Subscribe();
        _animates.StateChanged = ShowChange;
        _animates.LongestTravelTime = 200;
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
    private S? AnimatePiece { get; set; }
    private AnimateGrid _animates;
    async Task IHandleAsync<AnimatePieceEventModel<S>>.HandleAsync(AnimatePieceEventModel<S> message)
    {
        AnimatePiece = message.TemporaryObject!;
        if (AnimatePiece is ISelectableObject selects)
        {
            selects.IsSelected = false; //set to false.
        }
        _animates.LocationFrom = MainBoard!.GetControlLocation(message.PreviousSpace.Row, message.PreviousSpace.Column);
        _animates.LocationTo = MainBoard.GetControlLocation(message.MoveToSpace.Row, message.MoveToSpace.Column);
        await _animates.DoAnimateAsync();
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Unsubscribe();
                _animates.StateChanged = null;
                _animates = default!;
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
