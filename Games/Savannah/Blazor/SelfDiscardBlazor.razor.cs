namespace Savannah.Blazor;
public partial class SelfDiscardBlazor
{
    [Parameter]
    public RenderFragment<RegularSimpleCard>? ChildContent { get; set; }
    [Parameter]
    public SelfDiscardCP? DiscardPile { get; set; }
    [CascadingParameter]
    public int TargetImageHeight { get; set; }
    private static double Divider => 3.7;
    private static int AdditionalSpacing => 0; //i can add to it if necessary.
    private bool IsDisabled => !DiscardPile!.IsEnabled;
    private static string GetContainerStyle() => "overflow-x: auto; margin-right: 10px;";
    private string GetSvgStyle() => $"height: {TargetImageHeight}vh";
    private BasicList<PointF> _points = new();
    private SizeF _viewBox = new();
    private string GetViewBox()
    {
        return $"0 0 {_viewBox.Width} {_viewBox.Height}";
    }
    private string GetColorStyle()
    {
        if (IsDisabled == false)
        {
            return "";
        }
        return $"color:{cc.LightGray.ToWebColor()}; border-color: {cc.LightGray.ToWebColor()}";
    }
    private void CalculateHandMax()
    {
        var currentPoint = new PointF(0, 0);
        RegularSimpleCard card = new();
        var defaultSize = card.DefaultSize;
        double extras = 0;
        for (int i = 0; i < DiscardPile!.Maximum; i++)
        {
            extras = defaultSize.Width / Divider;
            extras += AdditionalSpacing;
            currentPoint.X += (float)extras;
        }
        currentPoint.X += (float)extras;
        _viewBox = new SizeF(currentPoint.X + defaultSize.Width / 2, defaultSize.Height);
    }
    private async Task ClickAsync()
    {
        if (DiscardPile!.HandList.Count == 0)
        {
            await DiscardPile.DiscardEmptyAsync();
        }
    }
    protected override void OnParametersSet()
    {
        _points = new();
        if (DiscardPile!.HandList.Count == 0)
        {
            RegularSimpleCard image = new();
            SizeF size = new(image.DefaultSize.Width * 2, image.DefaultSize.Height);
            _viewBox = size;
        }
        else
        {
            PointF currentPoint = new(0, 0);
            SizeF defaultSize = DiscardPile.HandList.First().DefaultSize;
            for (int i = 0; i < DiscardPile!.Maximum; i++)
            {
                PointF nextPoint = new(currentPoint.X, currentPoint.Y);
                _points.Add(nextPoint);
                double extras;
                extras = defaultSize.Width / Divider; //hopefuly this works.
                extras += AdditionalSpacing;
                currentPoint.X += (float)extras;
            }
            CalculateHandMax();
        }
    }
}