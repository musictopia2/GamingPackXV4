namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
[CustomTag("Main")] //later can rethink how to make it easier for strongly typed names.
public partial class RawGameBoard : IDisposable, IHandle<RepaintEventModel>
{
    private IEventAggregator? _aggregator;
    private bool _disposedValue;
    protected override void OnInitialized()
    {
        if (UseBuiltInAnimations == true)
        {
            _aggregator = Resolver!.Resolve<IEventAggregator>();
            Subscribe();
        }
        base.OnInitialized();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected virtual bool UseBuiltInAnimations { get; } = true;
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public string TargetWidth { get; set; } = "";
    [Parameter]
    public float X { get; set; }
    [Parameter]
    public float Y { get; set; }
    [Parameter]
    public SizeF BoardSize { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    private string GetSvgStyle()
    {
        if (TargetHeight == "" && TargetWidth == "")
        {
            return "";
        }
        if (TargetHeight != "" && TargetWidth != "")
        {
            return "";
        }

        if (TargetHeight != "")
        {
            return $"height: {TargetHeight}";
        }
        return $"width: {TargetWidth}";
    }
    private string GetViewBox()
    {
        return $"0 0 {BoardSize.Width} {BoardSize.Height}";
    }
    protected virtual void ProtectedUnsubscribe()
    {
        if (UseBuiltInAnimations)
        {
            Unsubscribe();
        }
    }
    protected void RefreshBoard()
    {
        if (RepaintEventModel.UpdatePartOfBoard != null)
        {
            RepaintEventModel.UpdatePartOfBoard.Invoke();
            return;
        }
        InvokeAsync(() => StateHasChanged());
    }
    void IHandle<RepaintEventModel>.Handle(RepaintEventModel message)
    {
        RefreshBoard();
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (UseBuiltInAnimations == true)
                {
                    Unsubscribe();
                }
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