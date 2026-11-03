namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public partial class GridGameBoard<S>
     where S : class, IBasicSpace, new()
{
    
    [Parameter]
    public double TargetHeight { get; set; } = 90;


    [Parameter]
    public string PreserveAspectRatio { get; set; } = "xMinYMin meet";

    protected int HeaderFontSize { get; set; } = 30;

    [Parameter]
    [EditorRequired]
    public int TargetSpaceWidth { get; set; }
    [Parameter]
    [EditorRequired]
    public int TargetSpaceHeight { get; set; }

    [Parameter]
    public int HeaderWidth { get; set; } = 50;
    [Parameter]
    public int HeaderHeight { get; set; } = 40;
    [Parameter]
    public IBoardCollection<S>? SpaceList { get; set; }
    [Parameter]
    public RenderFragment? Canvas { get; set; }
    [Parameter]
    public RenderFragment<S>? ItemTemplate { get; set; }
    [Parameter]
    public BasicList<string> RowList { get; set; } = new();
    [Parameter]
    public BasicList<string> ColumnList { get; set; } = new();
    


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
        if (TargetHeight <= 0)
        {
            return false;
        }
        if (ColumnList.Count == 0 && RowList.Count == 0)
        {
            return SpaceList!.Any() && TargetSpaceHeight > 0;
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
    protected virtual bool CanAddControl(IBoardCollection<S> itemsSource, int row, int column) => true;
    internal string HeaderString(string value)
    {
        string boldText = "";
        if (CanHeadersBold)
        {
            boldText = "font-weight='bold'";
        }
        return $"<text x='50%' y='55%' font-family='Lato' font-size='{HeaderFontSize}px' {boldText} fill='White' dominant-baseline='middle' text-anchor='middle'>{value}</text>";
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

    private int DisplayHeight => ll1.GetHeight(TargetHeight);

    private double DisplayWidth =>
        DisplayHeight * (double)ImageWidth / ImageHeight;

    private string DisplayHeightPixels =>
        DisplayHeight.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px";

    private string DisplayWidthPixels =>
        DisplayWidth.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px";


}