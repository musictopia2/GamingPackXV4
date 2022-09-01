namespace Pinochle2Player.Blazor.Views;
public partial class Pinochle2PlayerMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private Pinochle2PlayerVMData? _vmData;
    private Pinochle2PlayerGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<Pinochle2PlayerVMData>();
        _gameContainer = aa.Resolver.Resolve<Pinochle2PlayerGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(Pinochle2PlayerVMData.NormalTurn))
           .AddLabel("Deck Count", nameof(Pinochle2PlayerVMData.DeckCount))
           .AddLabel("Trump", nameof(Pinochle2PlayerVMData.TrumpSuit))
           .AddLabel("Status", nameof(Pinochle2PlayerVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", false, nameof(Pinochle2PlayerPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", false, nameof(Pinochle2PlayerPlayerItem.TricksWon))
            .AddColumn("Current Score", false, nameof(Pinochle2PlayerPlayerItem.CurrentScore))
            .AddColumn("Total Score", false, nameof(Pinochle2PlayerPlayerItem.TotalScore));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand MeldCommand => DataContext!.MeldCommand!;
}