namespace DominosMexicanTrain.Blazor;
[CustomTag("Main")] //later can rethink how to make it easier for strongly typed names.
public partial class TrainStationBlazor : IHandle<RepaintEventModel>, IDisposable
{
    public Action? UpdateAnimation { get; set; }
    [Parameter]
    public TrainStationBoardProcesses? GameBoard { get; set; }
    [CascadingParameter]
    public DominosMexicanTrainMainViewModel? DataContext { get; set; }
    [Parameter]
    public int TargetWidth { get; set; } = 39;
    private IEventAggregator? _aggregator;
    private bool _disposedValue;
    protected override void OnInitialized()
    {
        _aggregator = aa.Resolver!.Resolve<IEventAggregator>();
        Subscribe();
        GameBoard!.DominoLocationNeeded = GetDominoPoint;
        base.OnInitialized();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    private bool IsSelf(int index)
    {
        return index == GameBoard!.Self;
    }
    private static SizeF GetActualSize()
    {
        return new SizeF(150, 51);
    }
    private static float FindCenter(float currentLocation, bool doubles)
    {
        var tempSize = GetActualSize();
        var widths = tempSize.Width;
        var imageSize = tempSize.Height;
        float adds;
        adds = widths - imageSize;
        adds /= 2;
        if (doubles == true)
        {
            adds = 0;
        }
        return currentLocation + adds;
    }
    private static float FindTopLeft(float position, bool doubles)
    {
        if (doubles == false)
        {
            return position;
        }
        var tempSize = GetActualSize();
        return position + tempSize.Width - tempSize.Height;
    }
    private static float GetWidths()
    {
        var tempSize = GetActualSize();
        return tempSize.Width;
    }
    private PointF GetDominoPoint(int index, int firstNumber, int secondNumber)
    {
        TrainInfo train = GameBoard!.TrainList[index];
        bool doubles = firstNumber == secondNumber;
        return GetDominoPoint(train, doubles);
    }
    private PointF GetDominoPoint(TrainInfo train, bool doubles)
    {
        PrivateTrain privateTrain = GameBoard!.PrivateList[train.Index];
        int nums;
        if ((train.DominoList.Count == 0) & (doubles == false))
        {
            nums = 1;
        }
        else if ((train.DominoList.Count == 1) & (doubles == true))
        {
            nums = 1;
        }
        else
        {
            nums = 2;
        }
        return GetDominoPoint(privateTrain, nums, doubles);
    }
    private bool Satisfy(int index) => GameBoard!.Satisfy == index;
    private async Task TrainClicked(int player)
    {
        await DataContext!.TrainClickedAsync(player);
    }
    private static PointF GetDominoPoint(PrivateTrain train, int whichOne, bool doubles)
    {
        //1 very top/left
        //2 very top/right
        //3 very right/top
        //4 very left/top
        //5 very bottom/right
        //6 very bottom left
        //7 very left/bottom
        //8 very left/top
        var size = train.DominoArea.Size;
        var point = train.DominoArea.Location;
        var widths = GetWidths();
        RectangleF area = new(point, size);
        if (train.IsRotated == true)
        {
            if (whichOne == 1)
            {
                if (train.IsOpposite == true)
                {
                    return new PointF(FindCenter(area.Location.X, doubles), area.Bottom - widths);
                }
                return new PointF(FindCenter(area.Location.X, doubles), FindTopLeft(area.Top, doubles));
            }
            if (train.IsOpposite == true)
            {
                return new PointF(FindCenter(area.Location.X, doubles), area.Location.Y);
            }
            return new PointF(FindCenter(area.Location.X, doubles), FindTopLeft(area.Bottom - widths, doubles));
        }
        if (whichOne == 1)
        {
            if (train.IsOpposite == true)
            {
                return new PointF(area.Right - widths, FindCenter(area.Location.Y, doubles));
            }
            return new PointF(FindTopLeft(area.Location.X, doubles), FindCenter(area.Location.Y, doubles));
        }
        if (train.IsOpposite == true)
        {
            return new PointF(area.Location.X, FindCenter(area.Location.Y, doubles));
        }
        return new PointF(FindTopLeft(area.Right - widths, doubles), FindCenter(area.Location.Y, doubles));
    }
    void IHandle<RepaintEventModel>.Handle(RepaintEventModel message)
    {
        if (UpdateAnimation != null)
        {
            UpdateAnimation.Invoke();
        }
        else
        {
            InvokeAsync(StateHasChanged);
        }
    }
    private string GetWidth => $"{TargetWidth}vw";
    private static float GetLongestSize => 150;
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
    //looks like if i set shouldrender to false, then the children don't render no matter what.
    //this means each piece has to be more smart about it now.
    //maybe okay because statehaschanged is only for other anyways.
}
