namespace DummyRummy.Core.ViewModels;
[InstanceGame]
public partial class DummyRummyMainViewModel : BasicCardGamesVM<RegularRummyCard>
{
    private readonly DummyRummyMainGameClass _mainGame;
    private readonly DummyRummyVMData _model;
    private readonly DummyRummyGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    public DummyRummyMainViewModel(CommandContainer commandContainer,
        DummyRummyMainGameClass mainGame,
        DummyRummyVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        DummyRummyGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _privateAutoResume = privateAutoResume;
        _model.Deck1.NeverAutoDisable = false;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        PossibleAutoResume();
        _model.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        _model.MainSets.SendEnableProcesses(this, () => false); //always disabled this time.
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private void PossibleAutoResume()
    {
        if (_gameContainer.TempSets.Count > 0)
        {
            var player = _gameContainer.PlayerList!.GetSelf();
            bool hadAny = false;
            foreach (var item in _gameContainer.TempSets)
            {
                var current = _model.TempSets.SetList[item.SetNumber - 1];
                var cards = item.Cards.GetNewObjectListFromDeckList(_gameContainer.DeckList);
                DeckRegularDict<RegularRummyCard> toAdd = [];
                foreach (var card in cards)
                {
                    if (player.MainHandList.ObjectExist(card.Deck))
                    {
                        player.MainHandList.RemoveObjectByDeck(card.Deck);
                        player.AdditionalCards.Add(card); //if i remove from hand, must add to additional cards so sends to other players properly.
                        hadAny = true;
                        toAdd.Add(card);
                    }
                }
                current.AddCards(toAdd);
            }
            if (hadAny)
            {
                _model.TempSets.PublicCount();
            }
        }
    }
    [Command(EnumCommandCategory.OutOfTurn)]
    public void Back() //you can put back even if its not your turn.
    {
        var thisList = _model.TempSets!.ListSelectedObjects();
        thisList.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            _model.TempSets.RemoveObject(thisCard.Deck);
        });
        DummyRummyPlayerItem thisPlayer = _mainGame!.PlayerList!.GetSelf();
        thisPlayer.MainHandList.AddRange(thisList);
        _mainGame.SortCards();
    }

    public bool CanLayDownSets()
    {
        if (_gameContainer!.AlreadyDrew == false)
        {
            return false;
        }
        return !_mainGame.SaveRoot!.SetsCreated;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task LayDownSetsAsync()
    {
        if (_mainGame!.SaveRoot!.PlayerWentOut > 0)
        {
            bool lats = _mainGame.CanLaterLayDown();
            if (lats == false)
            {
                return;
            }
            var thisCol1 = _mainGame.ListValidSets();
            await ProcessValidSetsAsync(thisCol1);
            return;
        }
        bool rets = _mainGame.HasInitialSet();
        if (rets == false)
        {
            _toast.ShowUserErrorToast("Sorry, you do not have the valid sets needed to go out");
            return;
        }
        var thisCol2 = _mainGame.ListValidSets();
        await ProcessValidSetsAsync(thisCol2);
    }
    protected override bool CanEnableDeck()
    {
        return !_gameContainer!.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (_mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message) == false)
        {
            _toast.ShowUserErrorToast(message);
            return;
        }
        if (pickUp == true)
        {
            await _mainGame.PickupFromDiscardAsync();
            return;
        }
        await _gameContainer.SendDiscardMessageAsync(deck);
        await _mainGame.DiscardAsync(deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    private bool _isProcessing;
    private async Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return;
        }
        _isProcessing = true;
        var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
        _model.TempSets!.AddCards(index, tempList);
        await _privateAutoResume.SaveStateAsync(_model);
        _isProcessing = false;
    }
    private async Task ProcessValidSetsAsync(BasicList<TempInfo> thisCol)
    {
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                var thisStr = await js1.SerializeObjectAsync(tempList);
                newList.Add(thisStr);
            }
            _model.TempSets!.ClearBoard(thisTemp.SetNumber);
            _mainGame!.CreateSet(thisTemp.CardList);
        });
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendSeveralSetsAsync(newList, "finishedsets");
        }
        await _mainGame!.FinishedSetsAsync();
    }
}