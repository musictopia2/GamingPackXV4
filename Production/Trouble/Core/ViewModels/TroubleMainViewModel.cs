namespace Trouble.Core.ViewModels;
[InstanceGame]
public class TroubleMainViewModel(CommandContainer commandContainer,
        TroubleMainGameClass mainGame,
        TroubleVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        TroubleGameContainer gameContainer,
        IEventAggregator aggregator) : BoardDiceGameVM(commandContainer, mainGame, basicData, resolver, roller, aggregator)
{
    public TroubleVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        gameContainer.CanRollDice = CanRollDice;
        gameContainer.RollDiceAsync = RollDiceAsync;
        base.AdditionalInitialize();
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<TroublePlayerItem> PlayerList => mainGame.PlayerList;
    public override bool CanRollDice()
    {
        return mainGame.SaveRoot.DiceNumber == 0;
    }
    public override async Task RollDiceAsync() //if any changes, do here.
    {
        await base.RollDiceAsync();
    }
}