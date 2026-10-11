namespace Chess.Core.ViewModels;
[InstanceGame]
public partial class ChessMainViewModel(CommandContainer commandContainer,
        ChessMainGameClass mainGame,
        BasicData basicData,
         GameBoardProcesses gameBoard,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ChessVMData model) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public ChessVMData VMData { get; set; } = model;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanTie
    {
        get
        {
            if (mainGame.SaveRoot.SpaceHighlighted > 0)
            {
                return false;
            }
            return mainGame.SaveRoot.GameStatus != EnumGameStatus.EndingTurn;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task TieAsync()
    {
        if (BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("possibletie");
        }
        CommandContainer.ManuelFinish = true;
        await mainGame.ProcessTieAsync();
    }
    public bool CanUndoMoves => mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn;

    [Command(EnumCommandCategory.Game)]
    public async Task UndoMovesAsync()
    {
        if (BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("undomove");
        }
        await gameBoard.UndoAllMovesAsync();
    }
    public override bool CanEndTurn()
    {
        return mainGame!.SaveRoot!.GameStatus == EnumGameStatus.PossibleTie || mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn;
    }
}