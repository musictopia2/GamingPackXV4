namespace Countdown.Core.ViewModels;
[InstanceGame]
public partial class CountdownMainViewModel : DiceGamesVM<CountdownDice>
{
    private readonly CountdownMainGameClass _mainGame; //if we don't need, delete.
    public CountdownVMData VMData { get; set; }
    public CountdownMainViewModel(CommandContainer commandContainer,
        CountdownMainGameClass mainGame,
        CountdownVMData viewModel,
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
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<CountdownDice> GetCup => VMData.Cup!;
    public PlayerCollection<CountdownPlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return false; //if you can enable dice, change the routine.
    }
    public override bool CanEndTurn()
    {
        return true; //if you can't or has extras, do here.
    }
    public override bool CanRollDice()
    {
        return base.CanRollDice(); //anything you need to figure out if you can roll is here.
    }
    [Command(EnumCommandCategory.Game)]
    public void Hint()
    {
        if (VMData is null)
        {
            return; //so it does not have to be static.
        }
        CountdownVMData.ShowHints = true;
    }
}