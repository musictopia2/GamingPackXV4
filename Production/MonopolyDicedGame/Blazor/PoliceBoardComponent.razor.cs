namespace MonopolyDicedGame.Blazor;
public partial class PoliceBoardComponent
{
    [Parameter]
    public int PoliceUsed { get; set; }
    [Parameter]
    [EditorRequired]
    public double HeightPercent { get; set; }
    private string TargetHeight => HeightPercent.HeightPixels;

}