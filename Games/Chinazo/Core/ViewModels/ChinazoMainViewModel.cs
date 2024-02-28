namespace Chinazo.Core.ViewModels;
[InstanceGame]
public partial class ChinazoMainViewModel : BasicCardGamesVM<ChinazoCard>
{
    private readonly ChinazoMainGameClass _mainGame;
    private readonly ChinazoVMData _model;
    private readonly ChinazoGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    public ChinazoMainViewModel(CommandContainer commandContainer,
        ChinazoMainGameClass mainGame,
        ChinazoVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ChinazoGameContainer gameContainer,
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
        _model.Deck1.NeverAutoDisable = true;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        PossibleAutoResume();
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
                DeckRegularDict<ChinazoCard> toAdd = [];
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
    private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            return;
        }
        var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
        bool rets;
        rets = _mainGame!.CanAddToSet(thisSet, out ChinazoCard? thisCard, section, out string message);
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
            thiss.Position = section;
            await _mainGame.Network!.SendAllAsync("expandrummy", thiss);
        }
        await _mainGame.AddToSetAsync(nums, thisCard!.Deck, section);
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
    public bool CanTake => !CanEnablePile1();
    [Command(EnumCommandCategory.Game)]
    public async Task TakeAsync()
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
        var thisCol = _mainGame.ListValidSets();
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                SendNewSet thiss = new();
                thiss.CardListData = await js1.SerializeObjectAsync(thisTemp.CardList.GetDeckListFromObjectList());
                thiss.UseSecond = thisTemp.UseSecond;
                thiss.WhatSet = thisTemp.WhatSet;
                var thisStr = await js1.SerializeObjectAsync(thiss);
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
}