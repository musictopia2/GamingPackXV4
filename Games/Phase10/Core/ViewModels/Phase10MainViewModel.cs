namespace Phase10.Core.ViewModels;
[InstanceGame]
public partial class Phase10MainViewModel : BasicCardGamesVM<Phase10CardInformation>
{
    private readonly Phase10MainGameClass _mainGame;
    private readonly Phase10VMData _model;
    private readonly Phase10GameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    public Phase10MainViewModel(CommandContainer commandContainer,
        Phase10MainGameClass mainGame,
        Phase10VMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        Phase10GameContainer gameContainer,
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
        _model.Deck1.NeverAutoDisable = true;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        PossibleAutoResume();
        _model.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        _model.MainSets.SetClickedAsync = MainSets_SetClickedAsync;
        _model.MainSets.SendEnableProcesses(this, () => _gameContainer!.AlreadyDrew);
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
                DeckRegularDict<Phase10CardInformation> toAdd = [];
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
    protected override bool CanEnableDeck()
    {
        return !_gameContainer!.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    public bool CanSkipPlayer() => _model.PlayerChosen != "";
    [Command(EnumCommandCategory.Game)]
    public async Task SkipPlayerAsync()
    {
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("playerskipped", _model.PlayerChosen);
        }
        await _mainGame.SkipPlayerAsync(_model.PlayerChosen);
    }
    public bool CanCompletePhase()
    {
        if (_gameContainer!.AlreadyDrew == false)
        {
            return false;
        }
        return !_mainGame.SaveRoot!.CompletedPhase;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task CompletePhaseAsync()
    {
        bool rets;
        rets = _mainGame!.DidCompletePhase(out int Manys);
        if (Manys == _model.TempSets!.TotalObjects + _mainGame.SingleInfo!.MainHandList.Count)
        {
            _toast.ShowUserErrorToast("Cannot complete the phase.  Otherwise; there is no card to discard");
            return;
        }
        if (rets == false)
        {
            _toast.ShowUserErrorToast("Sorry, you did not complete the phase");
            return;
        }
        var thisCol = _mainGame.ListValidSets();
        if (thisCol.Count > 2)
        {
            throw new CustomBasicException("Can not have more than 2 sets");
        }
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                SendNewSet thisSend = new();
                thisSend.CardListData = await js1.SerializeObjectAsync(thisTemp.CardList.GetDeckListFromObjectList());
                thisSend.WhatSet = thisTemp.WhatSet;
                string newStr = await js1.SerializeObjectAsync(thisSend);
                newList.Add(newStr);
            }
            _mainGame.CreateNewSet(thisTemp);
        });
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendSeveralSetsAsync(newList, "phasecompleted");
        }
        await _mainGame.ProcessCompletedPhaseAsync();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        bool rets = _mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message);
        if (rets == false)
        {
            if (message != "")
            {
                _model.PlayerHand1!.UnselectAllObjects();
                _model.TempSets!.UnselectAllCards(); //its best to just unselect everything if you can't discard.  that will solve some issues.
                _toast.ShowUserErrorToast(message); //because on tablets, its possible it can't show message
            }
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
    private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            return;
        }
        if (_mainGame!.SaveRoot!.CompletedPhase == false)
        {
            _toast.ShowUserErrorToast("Sorry, the phase must be completed before expanding onto a set");
            return;
        }
        var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
        int position = section;
        bool rets;
        rets = _mainGame.CanHumanExpand(thisSet, ref position, out Phase10CardInformation? thisCard, out string message);
        if (rets == false)
        {
            if (message != "")
            {
                _toast.ShowUserErrorToast(message);
            }
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            SendExpandedSet expands = new();
            expands.Deck = thisCard!.Deck;
            expands.Position = position;
            expands.Number = setNumber;
            await _mainGame.Network!.SendAllAsync("expandrummy", expands);
        }
        await _mainGame.ExpandHumanRummyAsync(setNumber, thisCard!.Deck, position);
    }
}