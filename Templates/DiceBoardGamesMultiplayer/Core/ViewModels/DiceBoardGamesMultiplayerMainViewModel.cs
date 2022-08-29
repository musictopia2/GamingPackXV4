namespace DiceBoardGamesMultiplayer.Core.ViewModels;
[InstanceGame]
public class DiceBoardGamesMultiplayerMainViewModel : BoardDiceGameVM
{
    private readonly DiceBoardGamesMultiplayerMainGameClass _mainGame; //if we don't need, delete.
    public DiceBoardGamesMultiplayerVMData VMData { get; set; }
    public DiceBoardGamesMultiplayerMainViewModel(CommandContainer commandContainer,
        DiceBoardGamesMultiplayerMainGameClass mainGame,
        DiceBoardGamesMultiplayerVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
    }
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