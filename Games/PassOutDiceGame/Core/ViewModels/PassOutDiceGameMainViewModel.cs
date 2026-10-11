namespace PassOutDiceGame.Core.ViewModels;
[InstanceGame]
public class PassOutDiceGameMainViewModel(CommandContainer commandContainer,
        PassOutDiceGameMainGameClass mainGame,
        PassOutDiceGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        PassOutDiceGameGameContainer gameContainer,
        GameBoardProcesses gameBoard,
        IEventAggregator aggregator) : BoardDiceGameVM(commandContainer, mainGame, basicData, resolver, roller, aggregator)
{
    public PassOutDiceGameVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        gameContainer.MakeMoveAsync = MakeMoveAsync;
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        gameContainer.MakeMoveAsync = null;
        return base.CloseAsync();
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<PassOutDiceGamePlayerItem> PlayerList => mainGame.PlayerList;
    private async Task MakeMoveAsync(int space)
    {
        if (gameBoard.IsValidMove(space) == false)
        {
            return;
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendMoveAsync(space);
        }
        await mainGame.MakeMoveAsync(space);
    }
    public override bool CanRollDice()
    {
        return false;
    }
    public override async Task RollDiceAsync()
    {
        await base.RollDiceAsync();
    }
}