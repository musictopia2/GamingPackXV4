namespace SixtySix2Player.Blazor.Views;
public partial class SixtySix2PlayerMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private SixtySix2PlayerVMData? _vmData;
    private SixtySix2PlayerGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<SixtySix2PlayerVMData>();
        _gameContainer = aa1.Resolver.Resolve<SixtySix2PlayerGameContainer>();
        _labels.AddLabel("Turn", nameof(SixtySix2PlayerVMData.NormalTurn))
            .AddLabel("Trump", nameof(SixtySix2PlayerVMData.TrumpSuit))
            .AddLabel("Deck Count", nameof(SixtySix2PlayerVMData.DeckCount))
            .AddLabel("Status", nameof(SixtySix2PlayerVMData.Status))
            .AddLabel("Bonus", nameof(SixtySix2PlayerVMData.BonusPoints));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(SixtySix2PlayerPlayerItem.ObjectCount))
            .AddColumn("Tricks Won", true, nameof(SixtySix2PlayerPlayerItem.TricksWon))
            .AddColumn("Score Round", true, nameof(SixtySix2PlayerPlayerItem.ScoreRound))
            .AddColumn("Game Points Round", true, nameof(SixtySix2PlayerPlayerItem.GamePointsRound))
            .AddColumn("Total Points Game", true, nameof(SixtySix2PlayerPlayerItem.GamePointsGame));
        base.OnInitialized();
    }
    private ICustomCommand OutCommand => DataContext!.GoOutCommand!;
    private ICustomCommand MarriageCommand => DataContext!.AnnouceMarriageCommand!;
}