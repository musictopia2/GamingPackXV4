namespace BlockElevenSolitaire.Blazor.Views;
public partial class BlockElevenSolitaireMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Score", nameof(BlockElevenSolitaireMainViewModel.Score))
             .AddLabel("Cards Left", nameof(BlockElevenSolitaireMainViewModel.CardsLeft));
        base.OnInitialized();
    }
    private BasicMultiplePilesCP<SolitaireCard> GetWastePiles()
    {
        WastePilesCP waste = (WastePilesCP)DataContext!.WastePiles1;
        var output = waste.Discards;
        return output!;
    }
}