namespace DeadDie96.Core.ViewModels;
[InstanceGame]
public class DeadDie96MainViewModel : DiceGamesVM<TenSidedDice>
{
    private readonly DeadDie96MainGameClass _mainGame; //if we don't need, delete.
    public DeadDie96VMData VMData { get; set; }
    public DeadDie96MainViewModel(CommandContainer commandContainer,
        DeadDie96MainGameClass mainGame,
        DeadDie96VMData viewModel,
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
    public DiceCup<TenSidedDice> GetCup => VMData.Cup!;
    public PlayerCollection<DeadDie96PlayerItem> PlayerList => _mainGame.PlayerList;
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