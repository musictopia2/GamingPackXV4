namespace Cribbage.Blazor;
public partial class ScoreUI
{
    [Parameter]
    public BasicList<ScoreInfo>? ScoreList { get; set; }
}