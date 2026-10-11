namespace A21DiceGame.Core.ViewModels;
[InstanceGame]
public class A21DiceGameMainViewModel(CommandContainer commandContainer,
        A21DiceGameMainGameClass mainGame,
        A21DiceGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator) : DiceGamesVM<SimpleDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public A21DiceGameVMData VMData { get; set; } = viewModel;
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<A21DiceGamePlayerItem> PlayerList => mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return false; //if you can enable dice, change the routine.
    }
    public override bool CanEndTurn()
    {
        if (mainGame.SingleInfo is null)
        {
            return false;
        }
        return mainGame!.SingleInfo.NumberOfRolls > 0;
    }
    public override bool CanRollDice()
    {
        return base.CanRollDice(); //anything you need to figure out if you can roll is here.
    }
}