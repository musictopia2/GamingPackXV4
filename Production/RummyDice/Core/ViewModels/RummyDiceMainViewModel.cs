namespace RummyDice.Core.ViewModels;
[InstanceGame]
public partial class RummyDiceMainViewModel(CommandContainer commandContainer,
        RummyDiceMainGameClass mainGame,
        RummyDiceVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public readonly RummyDiceMainGameClass MainGame = mainGame;
    public RummyDiceVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn()
    {
        return VMData!.RollNumber >= 2;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task BoardAsync()
    {
        if (BasicData.MultiPlayer == true)
        {
            await MainGame.Network!.SendAllAsync("boardclicked");
        }
        await MainGame.BoardProcessAsync();
    }
    public bool CanRoll => VMData.RollNumber < 4;
    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        if (MainGame.MainBoard1.HasSelectedDice() == true)
        {
            toast.ShowUserErrorToast("Need to either unselect the dice or use them.");
            return;
        }
        await MainGame.RollDiceAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task CheckAsync()
    {
        if (BasicData.MultiPlayer == true)
        {
            await MainGame.Network!.SendAllAsync("calculate");
        }
        await MainGame.DoCalculateAsync();
    }
}