namespace SinisterSix.Core.ViewModels;
[InstanceGame]
public partial class SinisterSixMainViewModel(CommandContainer commandContainer,
        SinisterSixMainGameClass mainGame,
        SinisterSixVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator,
        IToast toast) : DiceGamesVM<EightSidedDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public SinisterSixVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<EightSidedDice> GetCup => VMData.Cup!;
    public PlayerCollection<SinisterSixPlayerItem> PlayerList => mainGame.PlayerList;
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
        return mainGame!.SaveRoot!.RollNumber <= mainGame.SaveRoot.MaxRolls;
    }
    public bool CanRemoveDice => CanEndTurn();
    [Command(EnumCommandCategory.Game)]
    public async Task RemoveDiceAsync()
    {
        if (CanRemoveSelectedDice() == false)
        {
            toast.ShowUserErrorToast("Cannot remove dice that does not equal 6");
            return;
        };
        await mainGame.RemoveSelectedDiceAsync();
    }
}