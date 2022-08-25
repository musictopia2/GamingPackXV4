namespace BuncoDiceGame.Blazor.Views;
public partial class BuncoDiceGameMainView
{
    private BasicList<ScoreColumnModel> ScoreList { get; set; } = new();
    private BasicList<LabelGridModel> Details { get; set; } = new();
    protected override void OnInitialized()
    {
        ScoreList.Clear();
        ScoreList.AddColumn("Points", false, nameof(PlayerItem.Points))
            .AddColumn("Table", false, nameof(PlayerItem.Table))
            .AddColumn("Team", false, nameof(PlayerItem.Team))
            .AddColumn("Buncos", false, nameof(PlayerItem.Buncos))
            .AddColumn("Wins", false, nameof(PlayerItem.Wins))
            .AddColumn("Losses", false, nameof(PlayerItem.Losses));
        Details.AddLabel("# To Get", nameof(StatisticsInfo.NumberToGet))
            .AddLabel("Set", nameof(StatisticsInfo.Set))
            .AddLabel("Your Team", nameof(StatisticsInfo.YourTeam))
            .AddLabel("Your Points", nameof(StatisticsInfo.YourPoints))
            .AddLabel("Opponent Score", nameof(StatisticsInfo.OpponentScore))
            .AddLabel("Buncos", nameof(StatisticsInfo.Buncos))
            .AddLabel("Wins", nameof(StatisticsInfo.Wins))
            .AddLabel("Losses", nameof(StatisticsInfo.Losses))
            .AddLabel("Your Table", nameof(StatisticsInfo.YourTable))
            .AddLabel("Team Mate", nameof(StatisticsInfo.TeamMate))
            .AddLabel("Opponent 1", nameof(StatisticsInfo.Opponent1))
            .AddLabel("Opponent 2", nameof(StatisticsInfo.Opponent2))
            .AddLabel("Turn", nameof(StatisticsInfo.Turn))
            .AddLabel("Status", nameof(StatisticsInfo.Status));
        base.OnInitialized();
    }
    #region Commands
    private ICustomCommand BuncoCommand => DataContext!.BuncoCommand!;
    private ICustomCommand Has21Command => DataContext!.Human21Command!;
    private ICustomCommand RollCommand => DataContext!.RollCommand!;
    private ICustomCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    #endregion
    private static PlayerCollection<PlayerItem> GetPlayers()
    {
        BuncoDiceGameSaveInfo saves = aa.Resolver!.Resolve<BuncoDiceGameSaveInfo>();
        return saves.PlayerList;
    }
    private static StatisticsInfo GetStats()
    {
        BuncoDiceGameSaveInfo saves = aa.Resolver!.Resolve<BuncoDiceGameSaveInfo>();
        return saves.ThisStats;
    }
}