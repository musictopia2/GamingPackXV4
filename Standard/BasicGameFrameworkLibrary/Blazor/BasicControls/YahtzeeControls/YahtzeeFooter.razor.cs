namespace BasicGameFrameworkLibrary.Blazor.BasicControls.YahtzeeControls;
public partial class YahtzeeFooter
{
    [Parameter]
    public string Text { get; set; } = "";
    [Parameter]
    public string TotalScore { get; set; } = "";
    [Parameter]
    public int RowNumber { get; set; }
}