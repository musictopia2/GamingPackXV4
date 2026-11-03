namespace MonopolyCardGame.Blazor;
public partial class TemporaryCardsBlazor
{
    [Parameter]
    public HandObservable<MonopolyCardGameCardInformation>? Hand { get; set; }
    [Parameter]
    public RenderFragment<MonopolyCardGameCardInformation>? ChildContent { get; set; }
    [Parameter]
    public string TargetContainerSize { get; set; } = "";
    [Parameter]
    public int HeightPixels { get; set; }
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public double AdditionalSpacing { get; set; } = -5;
    [Parameter]
    public EnumHandList HandType { get; set; } = EnumHandList.Horizontal;

    private (double Width, double Height) GetCardSize()
    {
        var size = new MonopolyCardGameCardInformation().DefaultSize;
        // Match BaseDeckGraphics' viewBox, including its four-unit border.
        double scale = HeightPixels / (double)Math.Max(size.Width + 4, size.Height + 4);
        return ((size.Width + 4) * scale, (size.Height + 4) * scale);
    }

    private double GetStep()
    {
        var (width, height) = GetCardSize();
        double length = HandType == EnumHandList.Horizontal ? width : height;
        return Math.Max(1, length / Math.Max(1, Divider) + AdditionalSpacing);
    }

    private (double Width, double Height) GetContentSize()
    {
        var (width, height) = GetCardSize();
        // Maximum reserves space; it must never clip cards already in the pile.
        int count = Math.Max(1, Math.Max(Hand!.Maximum, Hand.HandList.Count));
        double extra = (count - 1) * GetStep();
        return HandType == EnumHandList.Horizontal
            ? (width + extra, height)
            : (width, height + extra);
    }

    private string GetContainerStyle()
    {
        var (width, height) = GetContentSize();
        if (HandType == EnumHandList.Horizontal)
        {
            string containerWidth = TargetContainerSize == ""
                ? FormattableString.Invariant($"{width}px") : TargetContainerSize;
            return $"display:inline-block; vertical-align:top; width:{containerWidth}; max-width:100%; overflow:auto;";
        }
        string containerHeight = TargetContainerSize == ""
            ? FormattableString.Invariant($"{height}px") : TargetContainerSize;
        return FormattableString.Invariant(
            $"display:inline-block; vertical-align:top; width:{width}px; height:{containerHeight}; overflow:auto;");
    }

    private string GetContentStyle()
    {
        var (width, height) = GetContentSize();
        return FormattableString.Invariant($"position:relative; width:{width}px; height:{height}px;");
    }

    private string GetCardStyle(int index)
    {
        var (width, height) = GetCardSize();
        double left = HandType == EnumHandList.Horizontal ? index * GetStep() : 0;
        double top = HandType == EnumHandList.Vertical ? index * GetStep() : 0;
        return FormattableString.Invariant(
            $"position:absolute; left:{left}px; top:{top}px; width:{width}px; height:{height}px; line-height:0;");
    }
}
