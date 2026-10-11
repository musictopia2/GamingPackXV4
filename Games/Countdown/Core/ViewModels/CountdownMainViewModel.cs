namespace Countdown.Core.ViewModels;
[InstanceGame]
public partial class CountdownMainViewModel(CommandContainer commandContainer,
        CountdownMainGameClass mainGame,
        CountdownVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator) : DiceGamesVM<CountdownDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public CountdownVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<CountdownDice> GetCup => VMData.Cup!;
    public PlayerCollection<CountdownPlayerItem> PlayerList => mainGame.PlayerList;
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
        CommandContainer.QueueRefreshAfterCommand(CountdownActions.Board);
    }
}