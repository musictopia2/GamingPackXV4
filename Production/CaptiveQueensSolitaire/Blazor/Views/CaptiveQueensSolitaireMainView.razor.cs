namespace CaptiveQueensSolitaire.Blazor.Views;
public partial class CaptiveQueensSolitaireMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Score", nameof(CaptiveQueensSolitaireMainViewModel.Score))
            .AddLabel("First Start Number", nameof(CaptiveQueensSolitaireMainViewModel.FirstNumber))
            .AddLabel("Second Start Number", nameof(CaptiveQueensSolitaireMainViewModel.SecondNumber));
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