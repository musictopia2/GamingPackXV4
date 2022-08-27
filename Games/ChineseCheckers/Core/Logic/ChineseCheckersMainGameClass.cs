namespace ChineseCheckers.Core.Logic;
[SingletonGame]
public class ChineseCheckersMainGameClass
    : SimpleBoardGameClass<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo, EnumColorChoice, int>
    , IBeginningColors<EnumColorChoice, ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>
    , IMiscDataNM, ISerializable
{
    public ChineseCheckersMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        ChineseCheckersVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        ChineseCheckersGameContainer gameContainer,
        GameBoardProcesses gameBoard,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _gameContainer = gameContainer;
        _gameBoard = gameBoard;
        _gameContainer.Model = model;
        _gameContainer.MakeMoveAsync = PrivateMoveAsync;
        SaveRoot.Init(_gameContainer);
    }
    private void EnableControls()
    {
        _gameContainer.Command.StopExecuting();
    }
    private async Task PrivateMoveAsync(int space)
    {
        if (SaveRoot!.PreviousSpace == space && _gameBoard.WillContinueTurn() == false)
        {
            SaveRoot.Instructions = "Choose a piece to move";
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendAllAsync("undomove");
            }
            await _gameBoard.UnselectPieceAsync();
            EnableControls();
            return;
        }
        else if (SaveRoot.PreviousSpace == space)
        {
            EnableControls();
            return;
        }
        if (_gameBoard!.IsValidMove(space) == false)
        {
            EnableControls();
            return;
        }
        if (SaveRoot.PreviousSpace == 0)
        {
            if (SingleInfo!.PieceList.Any(xx => xx == space) == false)
            {
                EnableControls();
                return;
            }
            SaveRoot.Instructions = "Select where to move to";
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendAllAsync("pieceselected", space);
            }
            await _gameBoard.HighlightItemAsync(space);
            return;
        }
        SaveRoot.Instructions = "Making Move";
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendMoveAsync(space);
        }
        await MakeMoveAsync(space);
    }
    private readonly ChineseCheckersGameContainer _gameContainer;
    private readonly GameBoardProcesses _gameBoard;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        BoardGameSaved();
        SaveRoot.Init(_gameContainer);
        _gameBoard.LoadSavedGame();
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
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SaveRoot!.ImmediatelyStartTurn = true;
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "undomove":
                await _gameBoard.UnselectPieceAsync();
                return;
            case "pieceselected":
                await _gameBoard.HighlightItemAsync(int.Parse(content));
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
            _gameBoard.StartTurn();
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                SaveRoot!.Instructions = "Choose a piece to move";
            }
            else
            {
                SaveRoot!.Instructions = $"Waitng for {SingleInfo.NickName} to take their turn";
            }
        }
        await ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(int space)
    {
        await _gameBoard.MakeMoveAsync(space);
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        if (PlayerList.DidChooseColors())
        {
            _gameContainer.Command.ManuelFinish = true; //i think in this case, yes.
        }
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        _gameBoard.ClearBoard(); //i think here.
        await EndTurnAsync();
    }
}