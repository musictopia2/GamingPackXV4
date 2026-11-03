namespace ShipCaptainCrew.Core.ViewModels;
[InstanceGame]
public class ShipCaptainCrewMainViewModel(CommandContainer commandContainer,
        ShipCaptainCrewMainGameClass mainGame,
        ShipCaptainCrewVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator) : DiceGamesVM<SimpleDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public ShipCaptainCrewVMData VMData { get; set; } = viewModel;
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<ShipCaptainCrewPlayerItem> PlayerList => mainGame.PlayerList;
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