namespace CousinRummy.Core.ViewModels;
[InstanceGame]
public partial class CousinRummyMainViewModel : BasicCardGamesVM<RegularRummyCard>
{
    private readonly CousinRummyMainGameClass _mainGame;
    private readonly CousinRummyVMData _model;
    private readonly CousinRummyGameContainer _gameContainer;
    private readonly IToast _toast;
    public CousinRummyMainViewModel(CommandContainer commandContainer,
        CousinRummyMainGameClass mainGame,
        CousinRummyVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        CousinRummyGameContainer gameContainer,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = false;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard(); //try this too.
        _model.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        _model.MainSets.SetClickedAsync = MainSets_SetClickedAsync;
        _model.MainSets.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.OtherTurn > 0)
            {
                return false;
            }
            return _mainGame.SingleInfo!.LaidDown;
        });
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return _mainGame!.OtherTurn == 0;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int counts = _model.PlayerHand1!.HowManySelectedObjects;
        int others = _model.TempSets!.HowManySelectedObjects;
        if (counts + others > 1)
        {
            _toast.ShowUserErrorToast("Sorry, you can only select one card to discard");
            return;
        }
        if (counts + others == 0)
        {
            _toast.ShowUserErrorToast("Sorry, you must select a card to discard");
            return;
        }
        int index;
        int deck;
        if (counts == 0)
        {
            index = _model.TempSets.PileForSelectedObject;
            deck = _model.TempSets.DeckForSelectedObjected(index);
        }
        else
        {
            deck = _model.PlayerHand1.ObjectSelected();
        }
        await _gameContainer!.SendDiscardMessageAsync(deck);
        await _mainGame.DiscardAsync(deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    private bool _isProcessing;
    private Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return Task.CompletedTask;
        }
        _isProcessing = true;
        var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
        _model.TempSets!.AddCards(index, tempList);
        _isProcessing = false;
        return Task.CompletedTask;
    }
    private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            return;
        }
        var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
        bool rets;
        rets = _mainGame!.CanAddToSet(thisSet, out RegularRummyCard? thisCard, out string message);
        if (rets == false)
        {
            if (message != "")
            {
                _toast.ShowUserErrorToast(message);
            }
            return;
        }
        int nums = setNumber;
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            SendExpandedSet thiss = new();
            thiss.Deck = thisCard!.Deck;
            thiss.Number = nums;
            await _mainGame.Network!.SendAllAsync("expandrummy", thiss);
        }
        await _mainGame.AddToSetAsync(nums, thisCard!.Deck);
    }
    public bool CanPass => !CanEnablePile1();
    [Command(EnumCommandCategory.Game)]
    public async Task PassAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("pass");
        }
        await _mainGame!.PassAsync();
    }
    public bool CanBuy => !CanEnablePile1();
    [Command(EnumCommandCategory.Game)]
    public async Task BuyAsync()
    {
        await _mainGame!.PickupFromDiscardAsync();
    }
    public bool CanFirstSets
    {
        get
        {
            if (_mainGame!.OtherTurn > 0)
            {
                return false;
            }
            return !_mainGame.SingleInfo!.LaidDown;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task FirstSetsAsync()
    {
        if (_mainGame!.CanLayDownInitialSets() == false)
        {
            _toast.ShowUserErrorToast("Sorry; you do not have the required sets yet");
            return;
        }
        var thisCol = _mainGame.ListValidSets(true);
        BasicList<string> newList = new();
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                var thisStr = await js.SerializeObjectAsync(tempList);
                newList.Add(thisStr);
            }
            _mainGame.CreateNewSet(thisTemp);
        });
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendSeveralSetsAsync(newList, "laiddowninitial");
        }
        await _mainGame!.LaidDownInitialSetsAsync();
    }
    public bool CanOtherSets
    {
        get
        {
            if (_mainGame!.OtherTurn > 0)
            {
                return false;
            }
            return _mainGame.SingleInfo!.LaidDown;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task OtherSetsAsync()
    {
        var thisCol = _mainGame!.ListValidSets(false);
        if (thisCol.Count == 0)
        {
            _toast.ShowUserErrorToast("Sorry; you do not have any more sets to put down");
            return;
        }
        BasicList<string> newList = new();
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                var thisStr = await js.SerializeObjectAsync(tempList);
                newList.Add(thisStr);
            }
            _mainGame.CreateNewSet(thisTemp);
        });
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendSeveralSetsAsync(newList, "laydownothers");
        }
        await _mainGame.LayDownOtherSetsAsync();
    }
}