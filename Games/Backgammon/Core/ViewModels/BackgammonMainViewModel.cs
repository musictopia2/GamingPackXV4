namespace Backgammon.Core.ViewModels;
[InstanceGame]
public partial class BackgammonMainViewModel(CommandContainer commandContainer,
        BackgammonMainGameClass mainGame,
        BackgammonVMData model,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        GameBoardProcesses gameBoard,
        BackgammonGameContainer gameContainer,
        IEventAggregator aggregator) : BoardDiceGameVM(commandContainer, mainGame, basicData, resolver, roller, aggregator)
{
    public BackgammonVMData VMData { get; set; } = model;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        gameContainer.MakeMoveAsync = MakeMoveAsync;
        base.AdditionalInitialize();
    }

    protected override Task CloseAsync()
    {
        gameContainer.MakeMoveAsync = null;
        return base.CloseAsync();
    }

    partial void CreateCommands(CommandContainer command);
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<BackgammonPlayerItem> PlayerList => mainGame.PlayerList;
    private async Task MakeMoveAsync(int space)
    {
        if (gameBoard.IsValidMove(space) == false)
        {
            return;
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendMoveAsync(GameBoardProcesses.GetReversedID(space));
        }
        await mainGame.MakeMoveAsync(space);
    }
    public override bool CanEndTurn()
    {
        return mainGame!.SaveRoot!.GameStatus == EnumGameStatus.EndingTurn;
    }
    public override bool CanRollDice()
    {
        return false;
    }
    public override async Task RollDiceAsync()
    {
        await base.RollDiceAsync();
    }
    public bool CanUndoMove
    {
        get
        {
            if (mainGame.SaveRoot.SpaceHighlighted > -1)
            {
                return false;
            }
            return mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn || mainGame.SaveRoot.MadeAtLeastOneMove;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task UndoMoveAsync()
    {
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("undomove");
        }
        await gameBoard.UndoAllMovesAsync();
    }
}