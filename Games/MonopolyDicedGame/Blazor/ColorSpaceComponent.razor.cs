namespace MonopolyDicedGame.Blazor;
public partial class ColorSpaceComponent
{
    [Parameter]
    public string Color { get; set; } = cc1.White.ToWebColor; //default to white.
    [Parameter]
    public string Border { get; set; } = cc1.Black.ToWebColor;
    [Parameter]
    [EditorRequired]
    public double HeightPercent { get; set; }
    private string TargetImageHeight => HeightPercent.HeightPixels;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}