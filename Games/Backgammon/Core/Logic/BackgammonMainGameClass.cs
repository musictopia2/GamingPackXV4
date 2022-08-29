namespace Backgammon.Core.Logic;
[SingletonGame]
public class BackgammonMainGameClass
    : BoardDiceGameClass<BackgammonPlayerItem, BackgammonSaveInfo, EnumColorChoice, int>
    , IMiscDataNM, ISerializable
{
    private readonly BackgammonVMData _model;
    private readonly CommandContainer _command;
    private readonly GameBoardProcesses _gameBoard;
    private readonly BackgammonGameContainer _gameContainer;
    public BackgammonMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        BackgammonVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        BackgammonGameContainer container,
        StandardRollProcesses<SimpleDice, BackgammonPlayerItem> roller,
        GameBoardProcesses gameBoard,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller, error, toast)
    {
        _model = model;
        _command = command;
        _gameBoard = gameBoard;
        _gameContainer = container;
        _gameContainer.DiceVisibleProcesses = DiceVisibleProcesses;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        BoardGameSaved();
        _model.Cup!.ShowDiceListAlways = true;
        _model.Cup.Visible = true;
        _model.Cup.CanShowDice = true;
        SaveRoot.LoadMod(_model);
        if (PlayerList.DidChooseColors())
        {
            if (SaveRoot.SpaceHighlighted > 0 && BasicData.MultiPlayer && BasicData.Client == true)
            {
                SaveRoot.SpaceHighlighted = GameBoardProcesses.GetReversedID(SaveRoot.SpaceHighlighted); //try this way.
            }
            _gameBoard.ReloadSavedGame();
        }
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
    protected override Task ComputerTurnAsync()
    {
        throw new CustomBasicException("Computer should not go because it had too many problems");
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
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
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
            Cup!.DiceList.ForEach(thisDice => thisDice.Visible = true);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                Network!.IsEnabled = true;
                return;
            }
            await Roller!.RollDiceAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors() == false)
        {
            _model.LastStatus = "";
            await base.ContinueTurnAsync();
            return;
        }
        if (SaveRoot!.GameStatus == EnumGameStatus.EndingTurn)
        {
            _model.LastStatus = "Finished Moves";
            SaveRoot.Instructions = "Either End Turn Or Undo All Moves";
        }
        if (SaveRoot.MovesMade == 4)
        {
            _model.LastStatus = "Made Moves With Doubles.";
        }
        else if (SaveRoot.MovesMade > 0 && SaveRoot.SpaceHighlighted == -1)
        {
            _model.LastStatus = "Made At Least One Move";
        }
        else if (SaveRoot.SpaceHighlighted > -1)
        {
            if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn)
            {
                throw new CustomBasicException("It can't be ending turn if a space is highlighted");
            }
            _model.LastStatus = "";
            SaveRoot.Instructions = "Either Unhighlight space or finish move";
        }
        else
        {
            _model!.LastStatus = "";
            SaveRoot.Instructions = "Make Moves";
        }
        await base.ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(int space)
    {
        await _gameBoard.MakeMoveAsync(space);
    }
    public override async Task EndTurnAsync()
    {
        _command.ManuelFinish = true;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync() //risking no waiting after choosing colors (?)
    {
        SaveRoot!.GameStatus = EnumGameStatus.MakingMoves;
        _gameBoard.ClearBoard();
        await EndTurnAsync();
    }
    public override async Task AfterRollingAsync()
    {
        await _gameBoard.StartNewTurnAsync();
        await ContinueTurnAsync();
    }
    public void DiceVisibleProcesses()
    {
        var thisList = Cup!.DiceList;
        if (SaveRoot!.NumberUsed == 0 && SaveRoot.MovesMade == 0 && SaveRoot.MadeAtLeastOneMove == false)
        {
            thisList.ForEach(thisDice => thisDice.Visible = true);
            return;
        }
        if (_gameContainer!.HadDoubles())
        {
            if (SaveRoot.MovesMade < 2)
            {
                thisList.ForEach(thisDice => thisDice.Visible = true);
                return;
            }
            if (SaveRoot.MovesMade == 4)
            {
                thisList.ForEach(thisDice => thisDice.Visible = false);
                return;
            }
            thisList.First().Visible = false;
            return;
        }
        if (SaveRoot.MovesMade == 2)
        {
            thisList.ForEach(thisDice => thisDice.Visible = false);
            return;
        }
        if (SaveRoot.NumberUsed == _gameContainer.FirstDiceValue)
        {
            thisList.First().Visible = false;
        }
        else if (SaveRoot.NumberUsed == _gameContainer.SecondDiceValue)
        {
            thisList.Last().Visible = false;
        }
        else
        {
            throw new CustomBasicException("Not Sure");
        }
    }
}