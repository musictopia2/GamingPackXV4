namespace Trouble.Core.Logic;
[SingletonGame]
public class TroubleMainGameClass
    : BoardDiceGameClass<TroublePlayerItem, TroubleSaveInfo, EnumColorChoice, int>
    , ISerializable
{
    private readonly TroubleVMData _model;
    private readonly CommandContainer _command;
    private readonly GameBoardProcesses _gameBoard;
    public TroubleMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        TroubleVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        TroubleGameContainer container,
        StandardRollProcesses<SimpleDice, TroublePlayerItem> roller,
        GameBoardProcesses gameBoard,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller, error, toast)
    {
        _model = model;
        _command = command;
        _gameBoard = gameBoard;
        container.MakeMoveAsync = HumanMoveAsync;
    }
    private async Task HumanMoveAsync(int space)
    {
        if (BasicData.MultiPlayer)
        {
            await Network!.SendMoveAsync(space);
        }
        await MakeMoveAsync(space);
    }
    protected override void LoadCup(bool autoResume)
    {
        base.LoadCup(autoResume);
        if (Cup == null)
        {
            throw new CustomBasicException("Cup must be populated.  Rethink");
        }
        Cup.UpdateDiceAction = _gameBoard.Repaint;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        _gameBoard.LoadSavedGame();
        BoardGameSaved();
        SaveRoot.LoadMod(_model);
        _model.Cup!.CanShowDice = SaveRoot.DiceNumber > 0;
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
        SetUpDice();
        SaveRoot.LoadMod(_model);
        SaveRoot!.ImmediatelyStartTurn = true;
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
            _gameBoard.StartTurn();
        }
        await ContinueTurnAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            if (Test!.DoubleCheck)
            {
                Test.DoubleCheck = false;
                await _gameBoard.GetValidMovesAsync();
                return;
            }
        }
        await base.ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(int space)
    {
        await _gameBoard.MakeMoveAsync(space);
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        _command.ManuelFinish = true;
        if (PlayerList.DidChooseColors())
        {
            _gameBoard.StartTurn();
        }
        await StartNewTurnAsync();
    }

    public override async Task AfterChoosingColorsAsync()
    {
        _gameBoard.ClearBoard();
        await EndTurnAsync();
    }
    public override async Task AfterRollingAsync()
    {
        SaveRoot.DiceNumber = _model.Cup!.ValueOfOnlyDice;
        if (Test!.ImmediatelyEndGame)
        {
            await ShowWinAsync();
            return;
        }
        await _gameBoard.GetValidMovesAsync();
    }
}