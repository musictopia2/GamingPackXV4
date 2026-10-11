namespace BasicMultiplayerDiceGames.Core.ViewModels;
[InstanceGame]
public class BasicMultiplayerDiceGamesMainViewModel(CommandContainer commandContainer,
    BasicMultiplayerDiceGamesMainGameClass mainGame,
    BasicMultiplayerDiceGamesVMData viewModel,
    BasicData basicData,
    IGamePackageResolver resolver,
    IStandardRollProcesses roller,
    IEventAggregator aggregator
        ) : DiceGamesVM<SimpleDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    private readonly BasicMultiplayerDiceGamesMainGameClass _mainGame = mainGame; //if we don't need, delete.
    public BasicMultiplayerDiceGamesVMData VMData { get; set; } = viewModel;

    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<BasicMultiplayerDiceGamesPlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return false; //if you can enable dice, change the routine.
    }
    public override bool CanEndTurn()
    {
        return true; //if you can't or has extras, do here.
    }
    public override bool CanRollDice()
    {
        return base.CanRollDice(); //anything you need to figure out if you can roll is here.
    }
}