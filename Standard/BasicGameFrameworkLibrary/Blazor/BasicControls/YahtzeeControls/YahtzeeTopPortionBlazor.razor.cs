namespace BasicGameFrameworkLibrary.Blazor.BasicControls.YahtzeeControls;
public partial class YahtzeeTopPortionBlazor
{
    [Parameter]
    public BasicList<RowInfo> TopList { get; set; } = new();
    [Parameter]
    public RowInfo? BonusInfo { get; set; }
    [Parameter]
    public RowInfo? TopScore { get; set; }
    private int GetRow(RowInfo row) => TopList.IndexOf(row) + 2;
    [Parameter]
    public EventCallback<RowInfo> RowClicked { get; set; } //something else should handle this one.
}