namespace RageCardGame.Blazor.Views;
public partial class RageCardGameMainView
{
    private BasicList<ScoreColumnModel> _scores = new();
    private readonly BasicList<LabelGridModel> _labels = new();
    private RageCardGameVMData? _vmData;
    private RageCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<RageCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<RageCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(RageCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(RageCardGameVMData.Status))
            .AddLabel("Trump", nameof(RageCardGameVMData.TrumpSuit))
            .AddLabel("Lead", nameof(RageCardGameVMData.Lead));
        _scores = ScoreModule.GetScores();
        base.OnInitialized();
    }
}