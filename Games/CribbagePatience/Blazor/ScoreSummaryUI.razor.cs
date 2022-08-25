namespace CribbagePatience.Blazor;
public partial class ScoreSummaryUI
{
    [Parameter]
    public BasicList<int> Scores { get; set; } = new();
}