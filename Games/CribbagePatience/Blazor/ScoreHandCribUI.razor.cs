namespace CribbagePatience.Blazor;
public partial class ScoreHandCribUI
{
    [Parameter]
    public ScoreHandCP? Score { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 0;
    private string HeightString => $"{TargetHeight}vh";
}