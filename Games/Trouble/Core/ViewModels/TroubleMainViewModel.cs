namespace Trouble.Core.ViewModels;
[InstanceGame]
public class TroubleMainViewModel : BoardDiceGameVM
{
    private readonly TroubleMainGameClass _mainGame; //if we don't need, delete.
    public TroubleVMData VMData { get; set; }
    public TroubleMainViewModel(CommandContainer commandContainer,
        TroubleMainGameClass mainGame,
        TroubleVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        TroubleGameContainer gameContainer,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        gameContainer.CanRollDice = CanRollDice;
        gameContainer.RollDiceAsync = RollDiceAsync;
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<TroublePlayerItem> PlayerList => _mainGame.PlayerList;
    public override bool CanRollDice()
    {
        return _mainGame.SaveRoot.DiceNumber == 0;
    }
    public override async Task RollDiceAsync() //if any changes, do here.
    {
        await base.RollDiceAsync();
    }
}