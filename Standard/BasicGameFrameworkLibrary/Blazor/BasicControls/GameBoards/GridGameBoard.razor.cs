namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public partial class GridGameBoard<S>
     where S : class, IBasicSpace, new()
{
    [Parameter]
    public IBoardCollection<S>? SpaceList { get; set; } //i do need it to be a boardcollection this time for sure.
    [Parameter]
    public string PreserveAspectRatio { get; set; } = "xMinYMin meet";
    [Parameter]
    public RenderFragment? Canvas { get; set; }
    [Parameter]
    public RenderFragment<S>? ItemTemplate { get; set; }
    [Parameter]
    public int TargetSpaceHeight { get; set; } //this the other controls can use as well.
    [Parameter]
    public string GridHeight { get; set; } = "";
    [Parameter]
    public string GridWidth { get; set; } = ""; //to make it more flexible
    [Parameter]
    public int TargetSpaceWidth { get; set; } //this can be used as well.
    [Parameter]
    public BasicList<string> RowList { get; set; } = new();
    [Parameter]
    public BasicList<string> ColumnList { get; set; } = new();
    protected int HeaderFontSize { get; set; } = 30;
    [Parameter]
    public int HeaderWidth { get; set; } = 50;
    [Parameter]
    public int HeaderHeight { get; set; } = 40;
    private float ImageHeight
    {
        get
        {
            float output = TargetSpaceHeight * SpaceList!.GetTotalRows();
            if (ColumnList.Count == 0 && RowList.Count == 0)
            {
                return output;
            }
            return output + HeaderHeight;
        }
    }
    private float ImageWidth
    {
        get
        {
            float output = TargetSpaceWidth * SpaceList!.GetTotalColumns();
            if (ColumnList.Count == 0 && RowList.Count == 0)
            {
                return output;
            }
            return output + HeaderWidth;
        }
    }
    private bool CanInit()
    {
        if (ItemTemplate == null)
        {
            return false;
        }
        if (ColumnList.Count == 0 && RowList.Count == 0)
        {
            if (GridWidth != "" && GridHeight != "" && SpaceList!.Any() && TargetSpaceHeight > 0)
            {
                return true;
            }
            return false;
        }
        if (ColumnList.Count > 0 && RowList.Count == 0)
        {
            return false;
        }
        if (ColumnList.Count == 0 && RowList.Count > 0)
        {
            return false;
        }
        return true;
    }
    protected bool CanHeadersBold = true;
    protected virtual bool CanAddControl(IBoardCollection<S> itemsSource, int row, int column) => true; //defaults to true.
    internal string HeaderString(string value)
    {
        string boldText = "";
        if (CanHeadersBold)
        {
            boldText = "font-weight='bold'";
        }
        return $"<text x='50%' y='55%' font-family='tahoma' font-size='{HeaderFontSize}px' {boldText} fill='White' dominant-baseline='middle' text-anchor='middle'>{value}</text>";
    }
    public PointF GetControlLocation(int row, int column)
    {
        float startX;
        float startY;
        if (ColumnList.Count == 0 && RowList.Count == 0)
        {
            startX = 0;
            startY = 0;
        }
        else
        {
            startX = HeaderWidth;
            startY = HeaderHeight;
        }
        float nextx = TargetSpaceWidth * (column - 1);
        float nexty = TargetSpaceHeight * (row - 1);
        return new PointF(nextx + startX, nexty + startY);
    }
}