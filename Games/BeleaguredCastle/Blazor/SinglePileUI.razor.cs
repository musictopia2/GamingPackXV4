namespace BeleaguredCastle.Blazor;
public partial class SinglePileUI
{
    [CascadingParameter]
    public SolitairePilesCP? MainPiles { get; set; }
    [Parameter]
    public PileInfoCP? SinglePile { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private readonly BasicList<PointF> _points = new();
    protected override void OnParametersSet()
    {
        RecalculatePositions();
    }
    private SizeF _viewBox = new();
    private string GetViewBox()
    {
        return $"0 0 {_viewBox.Width} {_viewBox.Height}";
    }
    private async Task Submit()
    {
        if (MainPiles!.ColumnCommand!.CanExecute(SinglePile) == false)
        {
            return;
        }
        await MainPiles.ColumnCommand.ExecuteAsync(SinglePile);
    }
    private void RecalculatePositions()
    {
        _points.Clear();
        SolitaireCard image = new();
        float tempWidth = image.DefaultSize.Width * 6;
        SinglePile!.CardList.UnselectAllObjects();
        _viewBox = new SizeF(tempWidth, image.DefaultSize.Height);
        PointF currentPoint = new(0, 0);
        SizeF defaultSize = image.DefaultSize;
        double extras;
        foreach (var card in SinglePile.CardList)
        {
            PointF nextPoint = new(currentPoint.X, currentPoint.Y);
            _points.Add(nextPoint);
            extras = defaultSize.Width / 4;
            currentPoint.X += (float)extras;
        }
        _points.Reverse();
    }
}