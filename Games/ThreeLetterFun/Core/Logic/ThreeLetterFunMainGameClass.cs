namespace ThreeLetterFun.Core.Logic;
[SingletonGame]
public class ThreeLetterFunMainGameClass : BasicGameClass<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>
    , ICommonMultiplayer<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>
    , IMiscDataNM, ISerializable
{
    public ThreeLetterFunMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        ThreeLetterFunVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        GameBoard gameboard,
        GlobalHelpers global,
        IListShuffler<ThreeLetterFunCardData> deck,
        ThreeLetterFunGameContainer gameContainer,
        ISystemError error,
        IMessageBox message,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _test = test;
        _model = model;
        _command = command;
        GameBoard = gameboard;
        _global = global;
        _deck = deck;
        _message = message;
        _toast = toast;
        GameBoard.SetSelf = (() =>
        {
            SingleInfo = PlayerList.GetSelf();
        });
        GameBoard.SingleInfo = (() => SingleInfo!);
        GameBoard.SaveRoot = (() => SaveRoot);
    }
    private readonly TestOptions _test;
    private readonly ThreeLetterFunVMData _model;
    private readonly CommandContainer _command;
    public readonly GameBoard GameBoard;
    private readonly GlobalHelpers _global;
    private readonly IListShuffler<ThreeLetterFunCardData> _deck;
    private readonly IMessageBox _message;
    private readonly IToast _toast;

    private async Task GameOverAsync()
    {
        if (BasicData.MultiPlayer == false)
        {
            int counts = GameBoard.CardsRemaining();
            if (counts == 0)
            {
                _toast.ShowSuccessToast("Congratulations; you got rid of all 36 cards.  Therefore; you win");
            }
            else
            {
                _toast.ShowWarningToast($"{counts} cards left");
            }
            await this.ProtectedGameOverNextAsync();
            return;
        }
        SingleInfo = PlayerList.OrderByDescending(x => x.CardsWon).ThenBy(xx => xx.MostRecent).Take(1).Single();
        _model.PlayerWon = SingleInfo.NickName;
        await ShowWinAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        if (SaveRoot.CanStart == false)
        {
            return;
        }
        if (BasicData!.MultiPlayer == true)
        {
            SingleInfo = PlayerList!.GetSelf(); //because everybody takes their turn the same tie.
        }
        await SaveStateAsync();
        if (BasicData.MultiPlayer == false)
        {
            await ShowHumanCanPlayAsync();
            _model.TileBoard1!.ReportCanExecuteChange();
            GameBoard.ReportCanExecuteChange();
            return;
        }
        _command.ManuelFinish = true; //has to be manuel.  if you can play, not anymore.  has to be proven.
        SingleInfo = PlayerList!.GetSelf(); //because everybody takes their turn the same tie.
        if (SingleInfo.TookTurn == false)
        {
            await ShowHumanCanPlayAsync();
            _model.TileBoard1!.ReportCanExecuteChange(); //try this way now.
            GameBoard.ReportCanExecuteChange();//try this too.
            _global.Stops!.StartTimer();
            Network!.IsEnabled = false;
            return;
        }
        else
        {
            Network!.IsEnabled = true; //waiting to hear from other players.
        }
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.SavedList = GameBoard.ObjectList.ToRegularDeckDict();
        return Task.CompletedTask;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        _model.PlayerWon = "";
        if (SaveRoot!.CanStart == false)
        {
            await LoadStartOptionsAsync();
            return;
        }
        if (BasicData.MultiPlayer == true && SaveRoot.Level != EnumLevel.None)
        {
            _deck.OrderedObjects();
        }
        if (BasicData.MultiPlayer == true && SaveRoot.Level == EnumLevel.Easy)
        {
            PlayerList!.ForEach(player =>
            {
                player.MainHandList.ForEach(card => card.ReloadSaved());
            });
        }
        if (BasicData.MultiPlayer == false)
        {
            _deck.ClearObjects();
            _deck.OrderedObjects();
        }
        GameBoard.NewLoadSavedGame();
        GameBoard.Visible = true;
        _model.TileBoard1!.UpdateBoard();
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
        throw new CustomBasicException("Computer does not take a turn on single player games for this game");
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        //here has to wait.
        await _global.WaitAsync();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        if (BasicData.MultiPlayer == false)
        {
            SaveRoot.Level = EnumLevel.Hard;
            IShuffleTiles tiles = MainContainer.Resolve<IShuffleTiles>();
            await tiles.StartShufflingAsync(this);
        }
        else
        {
            SaveRoot.Level = EnumLevel.None;
            SaveRoot.CanStart = false;
            _model.PlayerWon = "";
            await LoadStartOptionsAsync();
        }
        await FinishUpAsync(isBeginning);
    }
    protected override void PrepStartTurn() { }
    public override Task StartNewTurnAsync()
    {
        return Task.CompletedTask;
    }
    public override Task EndTurnAsync()
    {
        throw new CustomBasicException("No Ending Turn.  Because the players takes their turns at the same time");
    }
    private async Task LoadStartOptionsAsync()
    {
        if (BasicData!.MultiPlayer == false)
        {
            throw new CustomBasicException("Single player should never load start options");
        }
        if (BasicData.Client == false)
        {
            await ShowHumanCanPlayAsync();
        }
        else
        {
            Network!.IsEnabled = true; //to wait for host to choose options.
        }
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning
    {
        get
        {
            if (BasicData.MultiPlayer == false)
            {
                return true;
            }
            return SaveRoot.CanStart;
        }
    }
    private async Task FinalAnalAsync()
    {
        _command.ManuelFinish = true;
        if (BasicData!.MultiPlayer == false)
        {
            throw new CustomBasicException("Single player cannot figure out the turns");
        }
        if (PlayerList.Any(items => items.TookTurn == false))
        {
            SingleInfo = PlayerList!.GetSelf();
            if (SingleInfo.TookTurn == false)
            {
                throw new CustomBasicException($"I think the player {SingleInfo.NickName} should have taken your turn before going through the last step");
            }
            Network!.IsEnabled = true; //waiting for others to show they took their turns
            return;
        }
        if (PlayerList.Any(Items => Items.TimeToGetWord == 0))
        {
            throw new CustomBasicException("Must have taken longer than 0 to get a word");
        }
        if (PlayerList.All(Items => Items.TimeToGetWord == -1))
        {
            await _message.ShowMessageAsync("Nobody found any words.  Therefore; going to the next one");
            await NextOneAsync();
            return;
        }
        if (BasicData.Client == true)
        {
            Network!.IsEnabled = true;
            return; //has to wait for host.
        }
        SingleInfo = PlayerList.Where(items => items.TimeToGetWord > -1).OrderBy
            (xx => xx.TimeToGetWord).Take(1).Single();
        WhoTurn = SingleInfo.Id;
        await Network!.SendAllAsync("whowon", WhoTurn);
        await ClientResultsAsync(WhoTurn);
    }
    private async Task NextOneAsync()
    {
        GameBoard.RemoveTiles();
        if (_test.ImmediatelyEndGame == false)
        {
            SaveRoot!.TileList.RemoveTiles(_model);
        }
        if (BasicData.MultiPlayer == true)
        {
            PlayerList!.TakeTurns();
        }
        if (SaveRoot!.TileList.Count == 0 || _test.ImmediatelyEndGame == true)
        {
            await GameOverAsync();
            return;
        }
        SaveRoot.UpTo++;
        await ContinueTurnAsync();
    }
    internal async Task ClientResultsAsync(int wins)
    {
        WhoTurn = wins;
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.TileList.Count == 0)
        {
            throw new CustomBasicException("You must have tiles if you won");
        }
        if (SingleInfo.CardUsed == 0)
        {
            throw new CustomBasicException("Don't know what card was used for the word for the player");
        }
        await GameBoard.ShowWordAsync(SingleInfo.CardUsed);
        if (GameBoard.CardsRemaining() == 0 || _test.ImmediatelyEndGame == true)
        {
            await GameOverAsync();
            return;
        }
        if (SaveRoot!.ShortGame == true && PlayerList.Any(Items => Items.CardsWon >= 5))
        {
            await GameOverAsync();
            return;
        }
        if (SaveRoot.Level == EnumLevel.Easy && PlayerList.Any(Items => Items.CardsWon >= SaveRoot.CardsToBeginWith))
        {
            await GameOverAsync();
            return;
        }
        await NextOneAsync();
    }
    public async Task GiveUpAsync()
    {
        if (BasicData.MultiPlayer == true)
        {
            SingleInfo = PlayerList!.GetWhoPlayer(); //i think this means the who turn has to be whoever gave up.
            SingleInfo.TookTurn = true;
            await _message.ShowMessageAsync($"{SingleInfo.NickName} took turn");
            SingleInfo.TimeToGetWord = -1;
            await FinalAnalAsync();
            return;
        }
        await NextOneAsync();
    }
    public async Task PlayWordAsync(int deck)
    {

        if (BasicData.MultiPlayer == false)
        {
            var thisCard = GameBoard.ObjectList.GetSpecificItem(deck);
            thisCard.Visible = false; //
            if (GameBoard.CardsRemaining() == 0 || _test.ImmediatelyEndGame == true)
            {
                await GameOverAsync();
                return;
            }
            await NextOneAsync();
            return;
        }
        SingleInfo!.TookTurn = true;
        SingleInfo.CardUsed = deck;
        await FinalAnalAsync();
    }
    //decided to have the main game class this time process the miscdata.
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "firstoption":
                await Aggregator.PublishAsync(new FirstOptionEventModel(content));
                break;
            case "advancedsettings":
                await Aggregator.PublishAsync(new AdvancedSettingsEventModel(content));
                break;
            case "howmanycards":
                await Aggregator.PublishAsync(new CardsChosenEventModel(int.Parse(content)));
                break;
            case "giveup": //no more tilelist now.
                SingleInfo = PlayerList!.GetSelf();
                if (SingleInfo.TookTurn == false)
                {
                    throw new CustomBasicException("Did not take turn");
                }
                SaveRoot!.PlayOrder.WhoTurn = int.Parse(content); //hopefully this works too.
                await GiveUpAsync();
                break;
            case "playword":
                SingleInfo = PlayerList!.GetSelf();
                if (SingleInfo.TookTurn == false)
                {
                    throw new CustomBasicException("Did not take turn");
                }
                TempWord thisWord = await js.DeserializeObjectAsync<TempWord>(content);
                SaveRoot!.PlayOrder.WhoTurn = thisWord.Player;
                SingleInfo = PlayerList.GetWhoPlayer(); //hopefully this still works.
                SingleInfo.TimeToGetWord = thisWord.TimeToGetWord;
                SingleInfo.TileList = thisWord.TileList;
                await PlayWordAsync(thisWord.CardUsed);
                break;
            case "whowon":
                await ClientResultsAsync(int.Parse(content));
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
}