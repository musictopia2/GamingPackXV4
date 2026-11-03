namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SolitaireClasses;

public partial class MainClockBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; }

    [Parameter]
    public ClockObservable? DataContext { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private int HeightPixels => ll1.GetHeight(TargetHeight);

    private string GetPositionStyle(ClockInfo clock)
    {
        SolitaireCard card = new();
        double scale = HeightPixels / (double)card.DefaultSize.Height;

        double top = scale * clock.Location.Y;
        double left = scale * clock.Location.X;

        return FormattableString.Invariant(
            $"top: {top}px; left: {left}px;");
    }
}