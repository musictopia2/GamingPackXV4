namespace LottoDominos.Core.Logic;
[SingletonGame]
public class LottoDominosMainGameClass : BasicGameClass<LottoDominosPlayerItem, LottoDominosSaveInfo>
    , ICommonMultiplayer<LottoDominosPlayerItem, LottoDominosSaveInfo>
    , IMiscDataNM, IMoveNM
{
    public LottoDominosMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        LottoDominosVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        GameBoardCP gameBoard,
        LottoDominosGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _test = test;
        _model = model;
        _gameBoard = gameBoard;
        _gameBoard.MakeMoveAsync = MakeMoveAsync;
    }
    private readonly TestOptions _test;
    private readonly LottoDominosVMData _model;
    private readonly GameBoardCP _gameBoard;
    #region "Delegates to stop the overflow"
    //because whoever has to reload the list may have to have access to the game class.
    //but if the game class required this too, that is overflow.  decided for now to keep the delegates
    public Action? ReloadNumberLists { get; set; }
    public Func<int, Task>? ProcessNumberAsync { get; set; }
    public Func<Task>? ComputerChooseNumberAsync { get; set; }
    #endregion
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        _gameBoard!.LoadSavedGame(SaveRoot!.BoardDice!);
        _model.DominosList!.ClearObjects(); //has to clear objects first.
        _model.DominosList.OrderedObjects(); //i think this should be fine.
        if (BasicData.Client == true)
        {
            SaveRoot.ComputerList.Clear(); //because they don't need to know about computer list.
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
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        PlayerList!.ForEach(items =>
        {
            items.NumberChosen = -1;
            items.NumberWon = 0;
        });
        _model.DominosList!.ClearObjects();
        _model.DominosList.ShuffleObjects(); //i think.
        SaveRoot!.ComputerList.Clear();
        SaveRoot.GameStatus = EnumStatus.ChooseNumber;
        _gameBoard.ClearPieces(); //i think
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        await FinishUpAsync(isBeginning);
    }
    public override async Task ContinueTurnAsync()
    {
        if (SaveRoot.GameStatus == EnumStatus.ChooseNumber)
        {
            if (ReloadNumberLists == null)
            {
                throw new CustomBasicException("Nobody is handling the reloading of the number lists.  Rethink");
            }
            ReloadNumberLists.Invoke();
        }
        await base.ContinueTurnAsync();
        if (SaveRoot.GameStatus == EnumStatus.NormalPlay)
        {
            _gameBoard.ReportCanExecuteChange();
        }
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.BoardDice = _gameBoard!.ObjectList.ToRegularDeckDict();
        return Task.CompletedTask;
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "numberchosen":
                int chosen = int.Parse(content);
                if (ProcessNumberAsync == null)
                {
                    throw new CustomBasicException("There was no function to process number chosen.  Rethink");
                }
                await ProcessNumberAsync.Invoke(chosen);
                break;

            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        this.ShowTurn();

        if (SingleInfo!.NumberChosen > -1 && SaveRoot.GameStatus == EnumStatus.ChooseNumber)
        {
            SaveRoot!.GameStatus = EnumStatus.NormalPlay;
            await Aggregator.PublishAsync(new ChangeGameStatusEventModel()); //i think should be here instead.
        }
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public BasicList<int> GetNumberList()
    {
        BasicList<int> output = new();
        for (int x = 0; x <= 6; x++)
        {
            if (PlayerList.Any(y => y.NumberChosen == x) == false)
            {
                output.Add(x);
            }
        }
        return output;
    }
    public async Task MoveReceivedAsync(string data)
    {
        await MakeMoveAsync(int.Parse(data));
    }
    public async Task MakeMoveAsync(int deck)
    {
        if (SingleInfo!.CanSendMessage(BasicData) == true)
        {
            await Network!.SendMoveAsync(deck);
        }
        await _gameBoard.ShowDominoAsync(deck);
        if (CanPlay(deck) == false)
        {
            AddComputer(deck);
            await EndTurnAsync();
            return;
        }
        TakeOffComputer(deck);
        _gameBoard.MakeInvisible(deck);
        SingleInfo!.NumberWon++;
        if (IsGameOver(SingleInfo.NumberWon) == true)
        {
            await ShowWinAsync();
            return;
        }
        await EndTurnAsync();
    }
    public bool CanPlay(int deck)
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer && _test!.AllowAnyMove == true)
        {
            return true; //because we are allowing any move for testing.
        }
        SimpleDominoInfo thisDomino = _model.DominosList!.GetSpecificItem(deck);
        return thisDomino.FirstNum == SingleInfo.NumberChosen || thisDomino.SecondNum == SingleInfo.NumberChosen;
    }
    private static bool IsGameOver(int score)
    {
        return score >= 4;
    }
    #region "Computer Processes"
    private void AddComputer(int deck)
    {
        if (BasicData.Client == true)
            return; //host does computer.
        if (SaveRoot!.ComputerList.ObjectExist(deck) == false)
        {
            SimpleDominoInfo thisDomino = _model.DominosList!.GetSpecificItem(deck);
            SaveRoot.ComputerList.Add(thisDomino);
        }
    }
    public void TakeOffComputer(int deck)
    {
        if (BasicData.Client == true)
        {
            return;
        }
        if (SaveRoot!.ComputerList.ObjectExist(deck) == false)
        {
            return;
        }
        SaveRoot.ComputerList.RemoveObjectByDeck(deck);
    }
    protected override async Task ComputerTurnAsync()
    {
        if (_test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SaveRoot!.GameStatus == EnumStatus.ChooseNumber)
        {
            if (ComputerChooseNumberAsync == null)
            {
                throw new CustomBasicException("Nobody is processing the number chosen for computer.  Rethink");
            }
            await ComputerChooseNumberAsync.Invoke();
            return;
        }
        DeckRegularDict<SimpleDominoInfo> output;
        if (SaveRoot.ComputerList.Count > 0)
        {
            output = SaveRoot.ComputerList.Where(xx => CanPlay(xx.Deck)).ToRegularDeckDict();
            if (output.Count > 0)
            {
                await MakeMoveAsync(output.GetRandomItem().Deck);
                return;
            }
        }
        output = _gameBoard.GetVisibleList();
        await MakeMoveAsync(output.GetRandomItem().Deck);
    }
    #endregion
}