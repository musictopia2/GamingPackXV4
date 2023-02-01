namespace Backgammon.Blazor;
public partial class BackgammonHintSpaceBlazor
{
    [Parameter]
    public RectangleF Bounds { get; set; }
    [Parameter]
    public string BorderColor { get; set; } = cc1.Transparent;
    [Parameter]
    public int SpaceNumber { get; set; } //this will determine what is rendered.
}