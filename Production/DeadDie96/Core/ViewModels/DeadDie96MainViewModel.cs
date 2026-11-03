namespace DeadDie96.Core.ViewModels;
[InstanceGame]
public class DeadDie96MainViewModel(CommandContainer commandContainer,
        DeadDie96MainGameClass mainGame,
        DeadDie96VMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator) : DiceGamesVM<TenSidedDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public DeadDie96VMData VMData { get; set; } = viewModel;
    public DiceCup<TenSidedDice> GetCup => VMData.Cup!;
    public PlayerCollection<DeadDie96PlayerItem> PlayerList => mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return false; //if you can enable dice, change the routine.
    }
    public override bool CanEndTurn()
    {
        return false; //if you can't or has extras, do here.
    }
    public override bool CanRollDice()
    {
        return base.CanRollDice(); //anything you need to figure out if you can roll is here.
    }
}