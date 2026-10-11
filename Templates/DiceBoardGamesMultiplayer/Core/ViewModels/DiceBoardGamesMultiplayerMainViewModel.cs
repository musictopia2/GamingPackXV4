namespace DiceBoardGamesMultiplayer.Core.ViewModels;
[InstanceGame]
public class DiceBoardGamesMultiplayerMainViewModel(CommandContainer commandContainer,
    DiceBoardGamesMultiplayerMainGameClass mainGame,
    DiceBoardGamesMultiplayerVMData viewModel,
    BasicData basicData,
    IGamePackageResolver resolver,
    IStandardRollProcesses roller,
    IEventAggregator aggregator
        ) : BoardDiceGameVM(commandContainer, mainGame, basicData, resolver, roller, aggregator)
{
    private readonly DiceBoardGamesMultiplayerMainGameClass _mainGame = mainGame; //if we don't need, delete.
    public DiceBoardGamesMultiplayerVMData VMData { get; set; } = viewModel;

    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<DiceBoardGamesMultiplayerPlayerItem> PlayerList => _mainGame.PlayerList;
    public override bool CanRollDice()
    {
        return base.CanRollDice();
    }
    public override async Task RollDiceAsync() //if any changes, do here.
    {
        await base.RollDiceAsync();
    }
}