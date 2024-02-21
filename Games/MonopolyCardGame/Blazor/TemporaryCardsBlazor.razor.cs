namespace MonopolyCardGame.Blazor;
public partial class TemporaryCardsBlazor
{
    [Parameter]
    public HandObservable<MonopolyCardGameCardInformation>? Hand { get; set; }
    [Parameter]
    public RenderFragment<MonopolyCardGameCardInformation>? ChildContent { get; set; }
    [Parameter]
    public string TargetContainerSize { get; set; } = ""; //if not set, will keep going forever.
    [Parameter]
    public string TargetImageSize { get; set; } = "";
    [CascadingParameter]
    public int TargetImageHeight { get; set; }
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public double AdditionalSpacing { get; set; } = -5;
    private string GetViewBox()
    {
        return $"0 0 {_viewBox.Width} {_viewBox.Height}";
    }

    private BasicList<PointF> _points = [];
    private SizeF _viewBox = new();
    private string GetContainerStyle()
    {
        if (TargetContainerSize == "")
        {
            return $"overflow-y: auto; margin-bottom: 10px; padding-right: 2px;";
        }
        return $"height: {TargetContainerSize}; overflow-y: auto; padding-right: 2px;";
    }
    private string GetSvgStyle()
    {
        if (TargetImageHeight > 0)
        {
            MonopolyCardGameCardInformation image = new();
            SizeF size = image.DefaultSize;
            var temps = TargetImageHeight * size.Width / size.Height;
            return $"width: {temps}vh";
        }
        if (TargetImageSize == "")
        {
            return "";
        }
        return $"width: {TargetImageSize}";
    }
    private void CalculateHandMax()
    {
        var currentPoint = new PointF(0, 0);
        MonopolyCardGameCardInformation card = new();
        var defaultSize = card.DefaultSize;
        double extras = 0;
        for (int i = 0; i < Hand!.Maximum; i++)
        {
            extras = defaultSize.Height / Divider;
            extras += AdditionalSpacing;
            currentPoint.Y += (float)extras;
        }
        currentPoint.Y += (float)extras;
        _viewBox = new SizeF(defaultSize.Width, currentPoint.Y);
    }
    protected override void OnParametersSet()
    {
        _points = [];
        if (Hand!.HandList.Count == 0)
        {
            MonopolyCardGameCardInformation image = new();
            if (Hand.Maximum == 0)
            {
                SizeF size = new(image.DefaultSize.Width * 2, image.DefaultSize.Height);
                _viewBox = size;
            }
            else
            {
                CalculateHandMax();
            }
        }
        else
        {
            PointF currentPoint = new(0, 0);
            SizeF defaultSize = new();
            foreach (var hand in Hand.HandList)
            {
                defaultSize = hand.DefaultSize;
                PointF nextPoint = new(currentPoint.X, currentPoint.Y);
                _points.Add(nextPoint);
                double extras;
                extras = hand.DefaultSize.Height / Divider;
                extras += AdditionalSpacing;

                currentPoint.Y += (float)extras;
            }
            if (Hand.Maximum == 0)
            {
                float maxX;
                if (Hand.HandList.Count == 1)
                {
                    maxX = defaultSize.Width;
                }
                else
                {
                    maxX = _points.Max(x => x.X);
                }
                float maxY = _points.Max(x => x.Y);
                _viewBox = new SizeF(defaultSize.Width, maxY + defaultSize.Height);
            }
            else
            {
                CalculateHandMax();
            }
        }
    }
}