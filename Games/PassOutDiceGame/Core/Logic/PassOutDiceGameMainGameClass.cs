namespace PassOutDiceGame.Core.Logic;
[SingletonGame]
public class PassOutDiceGameMainGameClass
    : BoardDiceGameClass<PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo, EnumColorChoice, int>
    , ISerializable
{
    private readonly PassOutDiceGameVMData _model;
    private readonly CommandContainer _command;
    private readonly GameBoardProcesses _gameBoard;
    public PassOutDiceGameMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        PassOutDiceGameVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        PassOutDiceGameGameContainer container,
        StandardRollProcesses<SimpleDice, PassOutDiceGamePlayerItem> roller,
        GameBoardProcesses gameBoard,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller, error, toast)
    {
        _model = model;
        _command = command;
        _gameBoard = gameBoard;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        BoardGameSaved();
        SaveRoot.LoadMod(_model);
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
    public override Task PopulateSaveRootAsync()
    {
        if (PlayerList.DidChooseColors() == true)
        {
            _gameBoard.SaveGame();
        }
        return base.PopulateSaveRootAsync();
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SetUpDice();
        SaveRoot.LoadMod(_model);
        SaveRoot.PreviousSpace = 0;
        SaveRoot!.ImmediatelyStartTurn = true;
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
            SaveRoot.DidRoll = false;
        }
        await ContinueTurnAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors() == false)
        {
            await base.ContinueTurnAsync();
        }
        if (SaveRoot.DidRoll == false)
        {
            if (BasicData.MultiPlayer && SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
            {
                Network!.IsEnabled = true;
                return;
            }
            SaveRoot.DidRoll = true;
            await Roller.RollDiceAsync();
            return;
        }
        await base.ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(int space)
    {
        _gameBoard.MakeMove(space);
        if (Test!.ImmediatelyEndGame)
        {
            await ShowWinAsync();
            return;
        }
        int wons = _gameBoard.WhoWon;
        if (wons > 0)
        {
            SingleInfo = PlayerList[wons]; //i think should be whoever actually won.
            await ShowWinAsync();
            return;
        }
        await EndTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        _command.ManuelFinish = true;
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        _gameBoard.ClearBoard();
        await EndTurnAsync();
    }
    public override async Task AfterRollingAsync()
    {
        SaveRoot.DidRoll = true;
        await ContinueTurnAsync();
    }
}