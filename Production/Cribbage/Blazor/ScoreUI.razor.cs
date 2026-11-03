namespace Cribbage.Blazor;
public partial class ScoreUI
{
    [Parameter]
    public StateList<ScoreInfo>? ScoreList { get; set; }
}