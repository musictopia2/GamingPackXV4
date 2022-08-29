namespace Aggravation.Core.ViewModels;
[InstanceGame]
public class AggravationMainViewModel : BoardDiceGameVM
{
    private readonly AggravationMainGameClass _mainGame; //if we don't need, delete.
    public AggravationVMData VMData { get; set; }
    public AggravationMainViewModel(CommandContainer commandContainer,
        AggravationMainGameClass mainGame,
        AggravationVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<AggravationPlayerItem> PlayerList => _mainGame.PlayerList;
    public override bool CanRollDice()
    {
        return _mainGame.SaveRoot.DiceNumber == 0;
    }
    public override async Task RollDiceAsync() //if any changes, do here.
    {
        await base.RollDiceAsync();
    }
}