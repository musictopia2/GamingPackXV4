namespace BasicGameFrameworkLibrary.Blazor.BasicControls.YahtzeeControls;
public partial class YahtzeeGameScoresheet<D>
    where D : SimpleDice, new()
{
    [Parameter]
    public ScoreContainer? ScoreContainer { get; set; }
    [CascadingParameter]
    public YahtzeeScoresheetViewModel<D>? DataContext { get; set; }
    [Parameter]
    public CommandContainer? CommandContainer { get; set; }
    [Parameter]
    public int BottomDescriptionWidth { get; set; }
    private async Task ProcessRowClickedAsync(RowInfo row)
    {
        if (DataContext!.CanRow(row) == false)
        {
            CommandContainer!.StopExecuting(); //try this.
            return;
        }
        await DataContext.RowAsync(row); //i think.
    }
    private RowInfo GetBonus()
    {
        RowInfo output;
        output = ScoreContainer!.RowList.Where(x => x.IsTop && x.RowSection == EnumRow.Bonus).Single();
        return output;
    }
    private RowInfo GetTopScore()
    {
        RowInfo output;
        output = ScoreContainer!.RowList.Where(x => x.IsTop && x.RowSection == EnumRow.Totals).Single();
        return output;
    }
    private BasicList<RowInfo> GetTopList()
    {
        BasicList<RowInfo> output;
        output = ScoreContainer!.RowList.Where(x => x.IsTop && x.RowSection == EnumRow.Regular).ToBasicList();
        return output;
    }
    private RowInfo GetBottomScore()
    {
        RowInfo output;
        output = ScoreContainer!.RowList.Where(x => x.IsTop == false && x.RowSection == EnumRow.Totals).Single();
        return output;
    }
    private BasicList<RowInfo> GetBottomList()
    {
        BasicList<RowInfo> output;
        output = ScoreContainer!.RowList.Where(x => x.IsTop == false && x.RowSection == EnumRow.Regular).ToBasicList();
        return output;
    }
}