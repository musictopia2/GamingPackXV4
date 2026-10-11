namespace RageCardGame.Blazor.Views;
public partial class RageBiddingView
{
    private ICustomCommand BidCommand => DataContext!.BidCommand!;
    private BasicList<ScoreColumnModel> _scores = [];
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnParametersSet()
    {
        _scores = ScoreModule.GetScores();
        _labels.Clear();
        _labels.AddLabel("Trump", nameof(RageBiddingViewModel.TrumpSuit))
            .AddLabel("Turn", nameof(RageBiddingViewModel.NormalTurn));
        base.OnParametersSet();
    }
}