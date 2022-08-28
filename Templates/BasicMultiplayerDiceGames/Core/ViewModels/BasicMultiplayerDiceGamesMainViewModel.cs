namespace BasicMultiplayerDiceGames.Core.ViewModels;
[InstanceGame]
public class BasicMultiplayerDiceGamesMainViewModel : DiceGamesVM<SimpleDice>
{
    private readonly BasicMultiplayerDiceGamesMainGameClass _mainGame; //if we don't need, delete.
    public BasicMultiplayerDiceGamesVMData VMData { get; set; }
    public BasicMultiplayerDiceGamesMainViewModel(CommandContainer commandContainer,
        BasicMultiplayerDiceGamesMainGameClass mainGame,
        BasicMultiplayerDiceGamesVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
    }
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