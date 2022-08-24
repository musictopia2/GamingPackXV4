namespace BasicGameFrameworkLibrary.Blazor.Animations;
public partial class AnimateCheckers
{
    [CascadingParameter]
    public PointF Location { get; set; }
    [Parameter]
    public EnumCheckerPieceCategory PieceCategory { get; set; }
    [Parameter]
    public int LongestSize { get; set; }
    [Parameter]
    public string Color { get; set; } = "";
}