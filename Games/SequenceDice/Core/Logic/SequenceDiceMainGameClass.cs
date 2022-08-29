namespace SequenceDice.Core.Logic;
[SingletonGame]
public class SequenceDiceMainGameClass
    : BoardDiceGameClass<SequenceDicePlayerItem, SequenceDiceSaveInfo, EnumColorChoice, SpaceInfoCP>
    , ISerializable
{
    private readonly SequenceDiceVMData _model;
    public SequenceDiceMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        SequenceDiceVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        SequenceDiceGameContainer container,
        StandardRollProcesses<SimpleDice, SequenceDicePlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller, error, toast)
    {
        _model = model;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        BoardGameSaved();
        if (SaveRoot.GameStatus == EnumGameStatusList.RollDice)
        {
            _model.Cup!.CanShowDice = false;
        }
        else
        {
            _model.Cup!.CanShowDice = true;
        }
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        SaveRoot.GameBoard.LoadBoard(PlayerList, Test!, _model); //this has to load.  because when rejoining, then does not work correctly.
        SaveRoot.LoadMod(_model);
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
        if (WhoTurn == 0)
        {
            throw new CustomBasicException("WhoTurn cannot be 0 at the start of the turn.");
        }
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
            SingleInfo = PlayerList!.GetWhoPlayer();
            SaveRoot!.GameStatus = EnumGameStatusList.RollDice;
            _model!.Cup!.CanShowDice = false;
        }
        await ContinueTurnAsync();
    }
    protected override void PrepStartTurn()
    {
        base.PrepStartTurn();
        SaveRoot!.GameBoard.StartTurn(WhoTurn);
    }
    public override Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            if (SaveRoot.GameStatus == EnumGameStatusList.RollDice)
            {
                SaveRoot.Instructions = "Roll the dice";
            }
            else if (SaveRoot.GameStatus == EnumGameStatusList.MovePiece)
            {
                if (Cup!.TotalDiceValue == 10)
                {
                    SaveRoot.Instructions = "Remove an opponents piece from the board.";
                }
                else if (Cup.TotalDiceValue == 11)
                {
                    SaveRoot.Instructions = "Put a piece on any open spaces on the board.  If there are none, then replace the opponent's piece with yours";
                }
                else
                {
                    SaveRoot.Instructions = "Put a piece on the number corresponding to the dice roll.  If there are no open spaces, then replace the opponent's piece with yours";
                }
            }
        }
        return base.ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(SpaceInfoCP space)
    {
        SpaceInfoCP newSpace = SaveRoot!.GameBoard[space.Vector];
        SaveRoot.GameBoard.MakeMove(newSpace, SingleInfo!);
        if (SaveRoot.GameBoard.HasWon() == true || Test!.ImmediatelyEndGame)
        {
            await ShowWinAsync();
            return;
        }
        int Totals = _model.Cup!.TotalDiceValue;
        if (Totals == 2 || Totals == 12)
        {
            SaveRoot.GameStatus = EnumGameStatusList.RollDice;
            await ContinueTurnAsync();
            return;
        }
        await EndTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        SaveRoot.GameBoard.ClearBoard();
        await EndTurnAsync();
    }
    public override async Task AfterRollingAsync()
    {
        int totals = _model.Cup!.TotalDiceValue;
        if (SaveRoot!.GameBoard.HasValidMove() == false)
        {
            if (totals == 2 || totals == 12)
            {
                SaveRoot.Instructions = "No moves possible.  Take another turn";
                if (Test!.NoAnimations == false)
                {
                    await Delay!.DelaySeconds(1);
                }
                _model.Cup.CanShowDice = false;
                await ContinueTurnAsync();
                return;
            }
            SaveRoot.Instructions = "No moves possible.  Ending turn";
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(1);
            }
            await EndTurnAsync();
            return;
        }
        SaveRoot.GameStatus = EnumGameStatusList.MovePiece;
        await ContinueTurnAsync();
    }
}