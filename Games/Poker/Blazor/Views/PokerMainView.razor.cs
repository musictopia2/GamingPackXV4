namespace Poker.Blazor.Views;
public partial class PokerMainView
{
    private BasicList<LabelGridModel> Labels { get; set; } = new();
    protected override void OnInitialized()
    {
        Labels.Clear();
        Labels.AddLabel("Money", nameof(PokerMainViewModel.Money))
            .AddLabel("Round", nameof(PokerMainViewModel.Round))
            .AddLabel("Winnings", nameof(PokerMainViewModel.Winnings))
            .AddLabel("Hand", nameof(PokerMainViewModel.HandLabel));
        //if you have to add command change, do so as well.
        base.OnInitialized();
    }
}