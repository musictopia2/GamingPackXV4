namespace GrandfathersClock.Blazor.Views;
public partial class GrandfathersClockMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Score", nameof(GrandfathersClockMainViewModel.Score)); //if there are others, do here.
        base.OnInitialized();
    }
    private CustomMain GetMainPiles()
    {
        CustomMain main = (CustomMain)DataContext!.MainPiles1;
        return main;
    }
    private SolitairePilesCP GetWastePiles()
    {
        WastePilesCP waste = (WastePilesCP)DataContext!.WastePiles1;
        var output = waste.Piles;
        return output;
    }
}