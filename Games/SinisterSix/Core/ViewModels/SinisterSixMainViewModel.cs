namespace SinisterSix.Core.ViewModels;
[InstanceGame]
public partial class SinisterSixMainViewModel : DiceGamesVM<EightSidedDice>
{
    private readonly SinisterSixMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public SinisterSixVMData VMData { get; set; }
    public SinisterSixMainViewModel(CommandContainer commandContainer,
        SinisterSixMainGameClass mainGame,
        SinisterSixVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<EightSidedDice> GetCup => VMData.Cup!;
    public PlayerCollection<SinisterSixPlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return true; //if you can enable dice, change the routine.
    }
    private bool CanRemoveSelectedDice()
    {
        var thisList = VMData.Cup!.DiceList.GetSelectedItems();
        return thisList.Sum(x => x.Value) == 6;
    }
    public override bool CanEndTurn()
    {
        return VMData.RollNumber > 0;
    }
    public override bool CanRollDice()
    {
        return _mainGame!.SaveRoot!.RollNumber <= _mainGame.SaveRoot.MaxRolls;
    }
    public bool CanRemoveDice => CanEndTurn();
    [Command(EnumCommandCategory.Game)]
    public async Task RemoveDiceAsync()
    {
        if (CanRemoveSelectedDice() == false)
        {
            _toast.ShowUserErrorToast("Cannot remove dice that does not equal 6");
            return;
        };
        await _mainGame.RemoveSelectedDiceAsync();
    }
}