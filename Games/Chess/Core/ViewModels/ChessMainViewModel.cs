namespace Chess.Core.ViewModels;
[InstanceGame]
public partial class ChessMainViewModel : BasicMultiplayerMainVM
{
    private readonly ChessMainGameClass _mainGame; //if we don't need, delete.
    public ChessVMData VMData { get; set; }
    private readonly BasicData _basicData;
    private readonly GameBoardProcesses _gameBoard;
    public ChessMainViewModel(CommandContainer commandContainer,
        ChessMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
         GameBoardProcesses gameBoard,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ChessVMData model
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        _basicData = basicData;
        _gameBoard = gameBoard;
        VMData = model;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanTie
    {
        get
        {
            if (_mainGame.SaveRoot.SpaceHighlighted > 0)
            {
                return false;
            }
            return _mainGame.SaveRoot.GameStatus != EnumGameStatus.EndingTurn;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task TieAsync()
    {
        if (_basicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("possibletie");
        }
        CommandContainer.ManuelFinish = true;
        await _mainGame.ProcessTieAsync();
    }
    public bool CanUndoMoves => _mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn;

    [Command(EnumCommandCategory.Game)]
    public async Task UndoMovesAsync()
    {
        if (_basicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("undomove");
        }
        await _gameBoard.UndoAllMovesAsync();
    }
    public override bool CanEndTurn()
    {
        return _mainGame!.SaveRoot!.GameStatus == EnumGameStatus.PossibleTie || _mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn;
    }
}