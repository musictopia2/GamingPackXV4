namespace MonopolyDicedGame.Blazor;
public partial class PoliceBoardComponent
{
    [Parameter]
    public int PoliceUsed { get; set; }
    [Parameter]
    public string TargetImageHeight { get; set; } = "";
   
}