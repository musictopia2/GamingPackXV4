namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.MainRummySets;
public partial class BaseIndividualRummySetBlazor<SU, CO, RU, SE, T>
    where SU : IFastEnumSimple
        where CO : IFastEnumColorSimple
        where RU : class, IRummmyObject<SU, CO>, IDeckObject, new()
        where SE : SetInfo<SU, CO, RU, T>
{
    
    [CascadingParameter]
    public BaseMainRummySetsBlazor<SU, CO, RU, SE, T>? MainSet { get; set; }
    [Parameter]
    public RenderFragment<RU>? ChildContent { get; set; }
    [Parameter]
    public SE? DataContext { get; set; }
    [CascadingParameter]
    public int TargetImageHeight { get; set; }


    private string GetSvgStyle()
    {
        RU image = new();
        SizeF size = image.DefaultSize;

        double targetSize = ll1.GetHeight(TargetImageHeight);
        double width = targetSize * size.Width /
            Math.Max(size.Width, size.Height);

        return $"width: {width.ToString(
            System.Globalization.CultureInfo.InvariantCulture)}px";
    }
    private async Task BoardClicked()
    {
        await DataContext!.BoardSingleClickCommand!.ExecuteAsync(null);
    }
    private string GetViewBox()
    {
        return $"0 0 {_viewBox.Width} {_viewBox.Height}";
    }
    private BasicList<PointF> _points = new();
    private SizeF _viewBox = new();

    private void CalculateLayout()
    {
        _points = new();
        if (DataContext!.HandList.Count == 0)
        {
            RU image = new();
            _viewBox = image.DefaultSize;
        }
        else
        {
            PointF currentPoint = new(0, 0);
            SizeF defaultSize = new();
            foreach (var hand in DataContext!.HandList)
            {
                defaultSize = hand.DefaultSize;
                PointF nextPoint = new(currentPoint.X, currentPoint.Y);
                _points.Add(nextPoint);
                double extras;
                extras = hand.DefaultSize.Height / MainSet!.Divider;
                extras += MainSet.AdditionalSpacing;
                currentPoint.Y += (float)extras;
            }
            float maxX = _points.Max(x => x.X);
            float maxY = _points.Max(x => x.Y);
            _viewBox = new SizeF(defaultSize.Width, maxY + defaultSize.Height);
        }
    }
    protected override void OnParametersSet()
    {
        CalculateLayout();
        
    }
}