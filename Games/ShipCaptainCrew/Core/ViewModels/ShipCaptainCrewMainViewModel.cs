namespace ShipCaptainCrew.Core.ViewModels;
[InstanceGame]
public class ShipCaptainCrewMainViewModel : DiceGamesVM<SimpleDice>
{
    private readonly ShipCaptainCrewMainGameClass _mainGame; //if we don't need, delete.
    public ShipCaptainCrewVMData VMData { get; set; }
    public ShipCaptainCrewMainViewModel(CommandContainer commandContainer,
        ShipCaptainCrewMainGameClass mainGame,
        ShipCaptainCrewVMData viewModel,
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
    public PlayerCollection<ShipCaptainCrewPlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return true; //if you can enable dice, change the routine.
    }
    public override bool CanEndTurn()
    {
        return false; //if you can't or has extras, do here.
    }
    public override bool CanRollDice()
    {
        return true;
    }
}