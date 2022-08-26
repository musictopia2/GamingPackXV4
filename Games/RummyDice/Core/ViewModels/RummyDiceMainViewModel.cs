namespace RummyDice.Core.ViewModels;
[InstanceGame]
public partial class RummyDiceMainViewModel : BasicMultiplayerMainVM
{
    public readonly RummyDiceMainGameClass MainGame; //if we don't need, delete.
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    public RummyDiceVMData VMData { get; set; }
    public RummyDiceMainViewModel(CommandContainer commandContainer,
        RummyDiceMainGameClass mainGame,
        RummyDiceVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        MainGame = mainGame;
        _basicData = basicData;
        _toast = toast;
        VMData = viewModel;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn()
    {
        return VMData!.RollNumber >= 2;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task BoardAsync()
    {
        if (_basicData!.MultiPlayer == true)
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
            _toast.ShowUserErrorToast("Need to either unselect the dice or use them.");
            return;
        }
        await MainGame.RollDiceAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task CheckAsync()
    {
        if (_basicData!.MultiPlayer == true)
        {
            await MainGame.Network!.SendAllAsync("calculate");
        }
        await MainGame.DoCalculateAsync();
    }
}