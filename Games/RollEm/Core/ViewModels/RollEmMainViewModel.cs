namespace RollEm.Core.ViewModels;
[InstanceGame]
public class RollEmMainViewModel(CommandContainer commandContainer,
        RollEmMainGameClass mainGame,
        RollEmVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        RollEmGameContainer gameContainer,
        IEventAggregator aggregator,
        GameBoardProcesses gameBoard,
        IToast toast) : DiceGamesVM<SimpleDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public RollEmVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        gameContainer.MakeMoveAsync = MakeMoveAsync;
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        CommandContainer.ExecutingChanged = null;
        gameContainer.MakeMoveAsync = null;
        return base.CloseAsync();
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<RollEmPlayerItem> PlayerList => mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return false;
    }
    public override bool CanEndTurn()
    {
        return mainGame!.SaveRoot!.GameStatus != EnumStatusList.NeedRoll;
    }
    public override bool CanRollDice()
    {
        return mainGame!.SaveRoot!.GameStatus == EnumStatusList.NeedRoll;
    }
    private async Task MakeMoveAsync(int space)
    {
        if (gameBoard.CanMakeMove(space) == false)
        {
            if (gameBoard.HadRecent)
            {
                if (BasicData.MultiPlayer)
                {
                    await mainGame.Network!.SendAllAsync("clearrecent");
                }
                toast.ShowUserErrorToast("Illegal Move");
                gameBoard.ClearRecent(true);
                await mainGame.ContinueTurnAsync();
            }
            return;
        }
        await mainGame.MakeMoveAsync(space);
    }
    private void CommandContainer_ExecutingChanged()
    {
        Aggregator.RepaintBoard();
    }
}