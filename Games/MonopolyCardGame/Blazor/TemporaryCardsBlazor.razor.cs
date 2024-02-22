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
    [Parameter]
    public int TargetImageHeight { get; set; }
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public double AdditionalSpacing { get; set; } = -5;

    [Parameter]
    public EnumHandList HandType { get; set; } = EnumHandList.Horizontal;


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
            if (HandType == EnumHandList.Horizontal)
            {
                return $"overflow-x: auto; margin-right: 10px;";
            }
            return $"overflow-y: auto; margin-bottom: 10px; padding-right: 3px;";
        }
        if (HandType == EnumHandList.Horizontal)
        {
            return $"width: {TargetContainerSize}; overflow-x: auto;";
        }
        return $"height: {TargetContainerSize}; overflow-y: auto; padding-right: 3px;";
    }
    private string GetSvgStyle()
    {
        if (HandType == EnumHandList.Horizontal && TargetImageHeight > 0)
        {
            return $"height: {TargetImageHeight}vh";
        }
        if (HandType == EnumHandList.Vertical && TargetImageHeight > 0)
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
        if (HandType == EnumHandList.Horizontal)
        {

            return $"height: {TargetImageSize}";
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
            if (HandType == EnumHandList.Horizontal)
            {
                extras = defaultSize.Width / Divider;
                extras += AdditionalSpacing;
                currentPoint.X += (float)extras;
            }
            else
            {
                extras = defaultSize.Height / Divider;
                extras += AdditionalSpacing;
                currentPoint.Y += (float)extras;
            }
        }
        if (HandType == EnumHandList.Horizontal)
        {
            currentPoint.X += (float)extras;
            _viewBox = new SizeF(currentPoint.X - defaultSize.Width + 10, defaultSize.Height);
        }
        else
        {
            currentPoint.Y += (float)extras;
            _viewBox = new SizeF(defaultSize.Width, currentPoint.Y);
        }
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
                if (HandType == EnumHandList.Horizontal)
                {
                    extras = hand.DefaultSize.Height / Divider;
                    extras += AdditionalSpacing;
                    currentPoint.X += (float)extras;
                }
                else
                {
                    extras = hand.DefaultSize.Width / Divider;
                    extras += AdditionalSpacing;

                    currentPoint.Y += (float)extras;
                }
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
                if (HandType == EnumHandList.Horizontal)
                {
                    _viewBox = new SizeF(maxX + defaultSize.Width, defaultSize.Height);
                }
                else
                {
                    _viewBox = new SizeF(defaultSize.Width, maxY + defaultSize.Height);
                }
                
            }
            else
            {
                CalculateHandMax();
            }
        }
    }
}