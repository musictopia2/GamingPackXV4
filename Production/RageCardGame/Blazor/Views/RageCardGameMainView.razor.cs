namespace RageCardGame.Blazor.Views;
public partial class RageCardGameMainView
{
    private BasicList<ScoreColumnModel> _scores = [];
    private readonly BasicList<LabelGridModel> _labels = [];
    private RageCardGameVMData? _vmData;
    private RageCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<RageCardGameVMData>();
        _gameContainer = Resolver.Resolve<RageCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(RageCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(RageCardGameVMData.Status))
            .AddLabel("Trump", nameof(RageCardGameVMData.TrumpSuit))
            .AddLabel("Lead", nameof(RageCardGameVMData.Lead));
        _scores = ScoreModule.GetScores();
        base.OnInitialized();
    }
}