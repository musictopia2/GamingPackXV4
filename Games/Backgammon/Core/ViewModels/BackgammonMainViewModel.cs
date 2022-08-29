namespace Backgammon.Core.ViewModels;
[InstanceGame]
public partial class BackgammonMainViewModel : BoardDiceGameVM
{
    private readonly BackgammonMainGameClass _mainGame; //if we don't need, delete.
    public BackgammonVMData VMData { get; set; }
    private readonly GameBoardProcesses _gameBoard;
    public BackgammonMainViewModel(CommandContainer commandContainer,
        BackgammonMainGameClass mainGame,
        BackgammonVMData model,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        GameBoardProcesses gameBoard,
        BackgammonGameContainer gameContainer,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = model;
        _gameBoard = gameBoard;
        gameContainer.MakeMoveAsync = MakeMoveAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<BackgammonPlayerItem> PlayerList => _mainGame.PlayerList;
    private async Task MakeMoveAsync(int space)
    {
        if (_gameBoard.IsValidMove(space) == false)
        {
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendMoveAsync(GameBoardProcesses.GetReversedID(space));
        }
        await _mainGame.MakeMoveAsync(space);
    }
    public override bool CanEndTurn()
    {
        return _mainGame!.SaveRoot!.GameStatus == EnumGameStatus.EndingTurn;
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
            if (_mainGame.SaveRoot.SpaceHighlighted > -1)
            {
                return false;
            }
            return _mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn || _mainGame.SaveRoot.MadeAtLeastOneMove;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task UndoMoveAsync()
    {
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("undomove");
        }
        await _gameBoard.UndoAllMovesAsync();
    }
}