namespace Aggravation.Core.ViewModels;
[InstanceGame]
public class AggravationMainViewModel(CommandContainer commandContainer,
        AggravationMainGameClass mainGame,
        AggravationVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator) : BoardDiceGameVM(commandContainer, mainGame, basicData, resolver, roller, aggregator)
{
    public AggravationVMData VMData { get; set; } = viewModel;
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<AggravationPlayerItem> PlayerList => mainGame.PlayerList;
    public override bool CanRollDice()
    {
        return mainGame.SaveRoot.DiceNumber == 0;
    }
    public override async Task RollDiceAsync() //if any changes, do here.
    {
        await base.RollDiceAsync();
    }
}