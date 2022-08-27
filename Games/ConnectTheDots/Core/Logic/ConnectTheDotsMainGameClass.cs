namespace ConnectTheDots.Core.Logic;
[SingletonGame]
public class ConnectTheDotsMainGameClass
    : SimpleBoardGameClass<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo, EnumColorChoice, int>
    , IBeginningColors<EnumColorChoice, ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>
    , ISerializable
{
    public ConnectTheDotsMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        ConnectTheDotsVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        ConnectTheDotsGameContainer container,
        GameBoardProcesses gameBoard,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, error, toast)
    {
        _command = command;
        _gameBoard = gameBoard;
        _toast = toast;
        container.MakeMoveAsync = PrivateMoveAsync;
    }
    private async Task PrivateMoveAsync(int dot)
    {
        if (_gameBoard.IsValidMove(dot) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            _command.StopExecuting();
            return;
        }
        if (BasicData.MultiPlayer)
        {
            await Network!.SendMoveAsync(dot);
        }
        await _gameBoard.MakeMoveAsync(dot);
    }
    private readonly CommandContainer _command;
    private readonly GameBoardProcesses _gameBoard;
    private readonly IToast _toast;

    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        BoardGameSaved();
        if (PlayerList.DidChooseColors())
        {
            _gameBoard.LoadGame();
        }
        return Task.CompletedTask;
    }
    public override Task PopulateSaveRootAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            _gameBoard.SaveGame();
        }
        return base.PopulateSaveRootAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
            return;

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
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
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
            _command.ManuelFinish = true;
        }
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        _gameBoard.ClearBoard();
        await EndTurnAsync();
    }
}