namespace BasicGameFrameworkLibrary.Blazor.BasicControls.RegularSpecialBoards;
public partial class RegularCardsPlayerBoardBlazor<T>
    where T : RegularTrickCard, IRegularCard, new()
{
    [Parameter]
    public PlayerBoardObservable<T>? DataContext { get; set; }
    private readonly BasicList<PointF> _points = new();
    private SizeF _size;
    public RegularCardsPlayerBoardBlazor()
    {
        T card = new();
        _size = card.DefaultSize;
    }
    private PointF GetCardLocation(int column, int row)
    {
        return new PointF(GetLeft(column), GetTop(row));
    }
    private float GetTop(int row)
    {
        var diffx = _size.Height / 2;
        return GetLocation(row, diffx);
    }
    private float GetLeft(int column)
    {
        var diffy = _size.Width;
        return GetLocation(column, diffy);
    }
    private static float GetLocation(int value, float diffs)
    {
        if (value == 1)
        {
            return 0;
        }
        return diffs * (value - 1);
    }
    private PointF GetCardLocation(T card)
    {
        var (row, column) = DataContext!.GetRowColumnData(card);
        return GetCardLocation(column, row);
    }
    private SizeF GetSize()
    {
        int totalRows = DataContext!.HowManyRows;
        int totalColumns = 4;
        float tempWidth = (_size.Width + 1) * totalColumns;
        float finalTop = GetTop(totalRows);
        return new SizeF(tempWidth, finalTop + _size.Height + 20);
    }
    private SizeF _viewBox;
    protected override void OnParametersSet()
    {
        if (DataContext == null)
        {
            return;
        }
        _points.Clear();
        foreach (var card in DataContext.CardList)
        {
            _points.Add(GetCardLocation(card));
        }
        _viewBox = GetSize();

        base.OnParametersSet();
    }
    private string GetViewBox()
    {
        return $"0 0 {_viewBox.Width} {_viewBox.Height}";
    }
    public string GetWidth()
    {
        int totals = TargetHeight * 4;
        return totals.WidthString<T>();
    }
    [CascadingParameter]
    public int TargetHeight { get; set; }
}