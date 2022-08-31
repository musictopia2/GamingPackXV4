namespace MilkRun.Blazor.Views;
public partial class MilkRunMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private MilkRunVMData? _vmData;
    private MilkRunGameContainer? _gameContainer;
    private BasicList<MilkRunPlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<MilkRunVMData>();
        _gameContainer = aa.Resolver.Resolve<MilkRunGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MilkRunVMData.NormalTurn));
        _players = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        _players.Reverse();
        base.OnInitialized();
    }
    private static string AnimationPileName(MilkRunPlayerItem player, EnumMilkType category)
    {
        return $"{category}{player.NickName}";
    }
}