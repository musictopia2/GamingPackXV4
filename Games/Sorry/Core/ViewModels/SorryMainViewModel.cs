namespace Sorry.Core.ViewModels;
[InstanceGame]
public class SorryMainViewModel : BasicMultiplayerMainVM
{
    private readonly SorryMainGameClass _mainGame;
    private readonly SorryGameContainer _gameContainer;
    private readonly GameBoardProcesses _gameBoard;
    public SorryVMData VMData { get; set; }
    public override bool CanEndTurn()
    {
        bool rets = base.CanEndTurn();
        if (rets == false)
        {
            return false;
        }
        return !_gameBoard.HasRequiredMove;
    }
    public SorryMainViewModel(CommandContainer commandContainer,
        SorryMainGameClass mainGame,
        SorryVMData model,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SorryGameContainer gameContainer,
        GameBoardProcesses gameBoard
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        _gameContainer = gameContainer;
        _gameBoard = gameBoard;
        VMData = model;
        _gameContainer.SpaceClickedAsync = MakeMoveAsync;
        _gameContainer.DrawClickAsync = DrawAsync;
        _gameContainer.HomeClickedAsync = HomeAsync;
    }
    private async Task MakeMoveAsync(int space)
    {
        if (_gameBoard.IsValidMove(space) == false)
        {
            _gameContainer.Command.StopExecuting(); //since command was not used now, this now has to be done.
            return;
        }
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendMoveAsync(space);
        }
        await _mainGame.MakeMoveAsync(space);
    }
    private async Task DrawAsync()
    {
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendDrawAsync();
        }
        await _mainGame.DrawCardAsync();
    }
    private async Task HomeAsync(EnumColorChoice color)
    {
        if (_gameBoard.CanGoHome(color) == false)
        {
            _gameContainer.Command.StopExecuting();
            return;
        }
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendMoveAsync(100);
        }
        await _mainGame.MakeMoveAsync(100);
    }
}