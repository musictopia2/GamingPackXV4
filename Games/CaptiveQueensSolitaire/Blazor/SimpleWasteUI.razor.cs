namespace CaptiveQueensSolitaire.Blazor;
public partial class SimpleWasteUI
{
    [CascadingParameter]
    public CaptiveQueensSolitaireMainViewModel? DataContext { get; set; }
    private CustomWaste? Waste { get; set; }
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
    private SizeF _viewBox = new();
    private readonly BasicList<PointF> _points = new();
    protected override void OnParametersSet()
    {
        Waste = (CustomWaste)DataContext!.WastePiles1;
        if (Waste.CardList.Count != 4)
        {
            return;
        }
        _points.Clear();
        var card = new SolitaireCard();
        var firstSize = card.DefaultSize;
        var tempSize = firstSize.Height * .67f;
        _points.Add(new PointF(0, tempSize));
        _points.Add(new PointF(tempSize, firstSize.Height + 10));
        _points.Add(new PointF(firstSize.Height, tempSize));
        _points.Add(new PointF(tempSize, 0));
        _viewBox.Height = card.DefaultSize.Height * 2;
        _viewBox.Width = _viewBox.Height;
        base.OnParametersSet();
    }
}