namespace A21DiceGame.Core.ViewModels;
[InstanceGame]
public class A21DiceGameMainViewModel : DiceGamesVM<SimpleDice>
{
    private readonly A21DiceGameMainGameClass _mainGame; //if we don't need, delete.
    public A21DiceGameVMData VMData { get; set; }
    public A21DiceGameMainViewModel(CommandContainer commandContainer,
        A21DiceGameMainGameClass mainGame,
        A21DiceGameVMData viewModel,
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
    public PlayerCollection<A21DiceGamePlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return false; //if you can enable dice, change the routine.
    }
    public override bool CanEndTurn()
    {
        return _mainGame!.SingleInfo!.NumberOfRolls > 0;
    }
    public override bool CanRollDice()
    {
        return base.CanRollDice(); //anything you need to figure out if you can roll is here.
    }
}