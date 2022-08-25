namespace FlorentineSolitaire.Blazor.Views;
public partial class FlorentineSolitaireMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Score", nameof(FlorentineSolitaireMainViewModel.Score))
            .AddLabel("Starting Number", nameof(FlorentineSolitaireMainViewModel.StartingNumber));
        base.OnInitialized();
    }
    private BasicMultiplePilesCP<SolitaireCard> GetMainPiles()
    {
        MainPilesCP main = (MainPilesCP)DataContext!.MainPiles1;
        var output = main.Piles;
        return output;
    }
    private BasicMultiplePilesCP<SolitaireCard> GetWastePiles()
    {
        WastePilesCP waste = (WastePilesCP)DataContext!.WastePiles1;
        var output = waste.Discards;
        return output!;
    }
}