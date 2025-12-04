namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SolitaireClasses;
public partial class IndividualSolitairePileBlazor
{
    protected override bool ShouldRender()
    {
        return _points.Count == SinglePile!.CardList.Count;
    }
    private string CustomKey => $"IndividualSolitairePile{SolitairePilesCP.DealNumber},{MainPiles!.PileList.IndexOf(SinglePile!)}";

    [CascadingParameter]
    public SolitairePilesCP? MainPiles { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public PileInfoCP? SinglePile { get; set; }
    private readonly BasicList<PointF> _points = new();
    protected override void OnParametersSet()
    {
        RecalculatePositions();
    }
    private void RecalculatePositions()
    {
        _points.Clear();
        if (SinglePile!.CardList.Count == 0)
        {
            SolitaireCard image = new();
            _viewBox = image.DefaultSize;
        }
        else
        {
            PointF currentPoint = new(0, 0);
            SizeF defaultSize = new();
            double extras;
            foreach (var card in SinglePile.CardList)
            {
                defaultSize = card.DefaultSize;
                PointF nextPoint = new(currentPoint.X, currentPoint.Y);
                _points.Add(nextPoint);
                if (card.IsUnknown == false || MainPiles!.IsKlondike == false)
                {
                    extras = defaultSize.Height / 4;
                }
                else
                {
                    extras = defaultSize.Height * .12;
                }
                extras += -5;
                currentPoint.Y += (float)extras;
            }
            _viewBox = new SizeF(defaultSize.Width, currentPoint.Y + defaultSize.Height);
        }
    }
    public string GetWidth => TargetHeight.WidthString<SolitaireCard>();
    private SizeF _viewBox = new();
    private string GetViewBox()
    {
        return $"0 0 {_viewBox.Width} {_viewBox.Height}";
    }
    private string GetFill()
    {
        if (SinglePile!.IsSelected)
        {
            return cs1.Red.ToWebColor;
        }
        return cs1.Transparent.ToWebColor;
    }
    private async Task ClickSinglePile()
    {
        if (MainPiles!.ColumnCommand!.CanExecute(SinglePile) == false)
        {
            return;
        }
        await MainPiles.ColumnCommand.ExecuteAsync(SinglePile);
    }
}