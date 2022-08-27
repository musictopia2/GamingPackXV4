namespace Chess.Core.Logic;
[SingletonGame]
public class ChessMainGameClass
    : SimpleBoardGameClass<ChessPlayerItem, ChessSaveInfo, EnumColorChoice, int>
    , IBeginningColors<EnumColorChoice, ChessPlayerItem, ChessSaveInfo>
    , IMiscDataNM, ISerializable, IFinishStart
{
    public ChessMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        ChessVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        GameBoardProcesses gameBoard,
        ChessGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _model = model;
        _gameBoard = gameBoard;
        _gameContainer = gameContainer;
        CheckersChessDelegates.CanMove = (() => SaveRoot.GameStatus == EnumGameStatus.None);
        CheckersChessDelegates.MakeMoveAsync = PrivateMakeMoveAsync;
    }
    private readonly ChessGameContainer _gameContainer;
    private async Task PrivateMakeMoveAsync(int space)
    {
        if (_gameBoard.IsValidMove(space) == false)
        {
            _gameContainer.Command.StopExecuting(); //maybe even better.
            return;
        }
        if (BasicData.MultiPlayer)
        {
            await Network!.SendMoveAsync(GameBoardGraphicsCP.GetRealIndex(space, true));
        }
        _gameContainer.Command.IsExecuting = true; //try this way.
        await _gameBoard.MakeMoveAsync(space);
    }

    private readonly ChessVMData _model;
    private readonly GameBoardProcesses _gameBoard;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        BoardGameSaved();
        _autoResume = true;
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        await Task.CompletedTask;
    }
    private bool _autoResume;
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SaveRoot!.ImmediatelyStartTurn = true;
        _autoResume = false;
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "possibletie":
                await ProcessTieAsync();
                return;
            case "undomove":
                await _gameBoard.UndoAllMovesAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
            await _gameBoard.StartNewTurnAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    public override Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            if (SaveRoot.GameStatus == EnumGameStatus.PossibleTie)
            {
                _model.Instructions = "Either Agree To Tie Or End Turn";
            }
            else if (SaveRoot.SpaceHighlighted == 0)
            {
                _model.Instructions = "Make Move Or Initiate Tie";
            }
            else
            {
                _model.Instructions = "Finish Move";
            }
        }
        return base.ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(int space)
    {
        await _gameBoard.MakeMoveAsync(space);
    }
    public override async Task EndTurnAsync()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.PossibleTie = false);
        if (PlayerList.DidChooseColors() && SaveRoot.PossibleMove != null)
        {
            SaveRoot.PreviousMove = SaveRoot.PossibleMove;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        SaveRoot!.GameStatus = EnumGameStatus.None;
        _gameBoard.ClearBoard();
        _gameContainer.CurrentMoveList.Clear();
        SaveRoot.PreviousMove = new PreviousMove();
        SaveRoot.PossibleMove = new PreviousMove();
        await EndTurnAsync();
    }
    async Task IFinishStart.FinishStartAsync()
    {
        if (_autoResume && PlayerList.DidChooseColors() == true)
        {
            await _gameBoard.ReloadSavedGameAsync();
        }
    }
    public async Task ProcessTieAsync()
    {
        SingleInfo!.PossibleTie = true;
        if (PlayerList.Any(items => items.PossibleTie == false))
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            SaveRoot!.GameStatus = EnumGameStatus.PossibleTie;
            SingleInfo = PlayerList.GetWhoPlayer();
            PrepStartTurn();
            await ContinueTurnAsync();
            return;
        }
        await ShowTieAsync();
    }
    public override Task ShowTieAsync()
    {
        Aggregator.RepaintBoard();
        _gameContainer.CanUpdate = false;
        return base.ShowTieAsync();
    }
    public override Task ShowWinAsync()
    {
        Aggregator.RepaintBoard();
        _gameContainer.CanUpdate = false;
        return base.ShowWinAsync();
    }
}