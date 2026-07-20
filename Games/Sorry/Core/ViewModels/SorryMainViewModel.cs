namespace Sorry.Core.ViewModels;
[InstanceGame]
public partial class SorryMainViewModel : BasicMultiplayerMainVM
{
    private readonly CommandContainer _commandContainer;
    private readonly SorryMainGameClass _mainGame;
    private readonly SorryGameContainer _gameContainer;
    private readonly GameBoardProcesses _gameBoard;
    private readonly TestOptions _test;

    private readonly AdvancedTestStateService _advancedState;
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
        GameBoardProcesses gameBoard,
        AdvancedTestStateService advancedState
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _commandContainer = commandContainer;
        _mainGame = mainGame;
        _gameContainer = gameContainer;
        _gameBoard = gameBoard;
        _advancedState = advancedState;
        _test = test;
        VMData = model;
        _gameContainer.SpaceClickedAsync = MakeMoveAsync;
        _gameContainer.DrawClickAsync = DrawAsync;
        _gameContainer.HomeClickedAsync = HomeAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public bool DidDraw => _mainGame.SaveRoot.DidDraw;
    private async Task MakeMoveAsync(int space)
    {

        if (_test!.AdvancedTestOptions)
        {
            AdvancedPlacePiece(space);
            await _mainGame.ContinueTurnAsync();
            //_command.StopExecuting();
            return;
        }



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
    private int _advancedSelectedPiece;

    public int AdvancedSelectedPiece
    {
        get => _advancedSelectedPiece;
        set
        {
            if (SetProperty(ref _advancedSelectedPiece, value))
            {
                _commandContainer.UpdateAll();
            }
        }
    }
    private void AdvancedPlacePiece(int space)
    {
        SorryPlayerItem player = _mainGame.SingleInfo!;

        if (AdvancedSelectedPiece == 0)
        {
            if (!player.PieceList.Contains(space))
            {
                return;
            }

            AdvancedSelectedPiece = space;
            return;
        }

        if (!_gameBoard.CanAdvancedPlace(player, space))
        {
            return;
        }

        player.PieceList.RemoveSpecificItem(AdvancedSelectedPiece);
        player.PieceList.Add(space);

        AdvancedSelectedPiece = 0;



        //_gameBoard.LoadSavedGame();
        //_gameContainer.RepaintBoard();
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
    [Command(EnumCommandCategory.Game)]
    public async Task LockDrawCardAsync()
    {
        if (_test.AdvancedTestOptions == false)
        {
            throw new CustomBasicException("Should only be able to lock draw card if advanced test options is enabled");
        }
        _mainGame.SaveRoot.DidDraw = true;
        _mainGame.SaveRoot.AdvancedCardPreview = false;
        await _mainGame.ContinueTurnAsync();
    }
}