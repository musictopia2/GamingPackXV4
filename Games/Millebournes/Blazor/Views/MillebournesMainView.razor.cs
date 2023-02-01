namespace Millebournes.Blazor.Views;
public partial class MillebournesMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private MillebournesVMData? _vmData;
    private MillebournesGameContainer? _gameContainer;
    private static string GetRows => "1fr 1fr 1fr";
    private static string GetColumns => "1fr 1fr";
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<MillebournesVMData>();
        _gameContainer = aa1.Resolver.Resolve<MillebournesGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MillebournesVMData.NormalTurn))
           .AddLabel("Status", nameof(MillebournesVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Team", true, nameof(MillebournesPlayerItem.Team))
            .AddColumn("Miles", true, nameof(MillebournesPlayerItem.Miles))
            .AddColumn("Cards", true, nameof(MillebournesPlayerItem.ObjectCount))
            .AddColumn("Other Points", true, nameof(MillebournesPlayerItem.OtherPoints))
            .AddColumn("Total Points", true, nameof(MillebournesPlayerItem.TotalPoints))
            .AddColumn("# 200s", true, nameof(MillebournesPlayerItem.Number200s));
        base.OnInitialized();
    }
    private static string GetAnimationTag(TeamCP team)
    {
        return $"team{team.TeamNumber}";
    }
}