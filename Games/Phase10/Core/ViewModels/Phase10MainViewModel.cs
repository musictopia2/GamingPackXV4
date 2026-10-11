namespace Phase10.Core.ViewModels;
[InstanceGame]
public partial class Phase10MainViewModel(CommandContainer commandContainer,
        Phase10MainGameClass mainGame,
        Phase10VMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        Phase10GameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<Phase10CardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        var player = mainGame.PlayerList.GetSelf();
        player.DoInit();
        viewModel.TempSets.Init(this);
        viewModel.TempSets.ClearBoard();
        PossibleAutoResume();
        viewModel.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        viewModel.MainSets.SetClickedAsync = MainSets_SetClickedAsync;
        viewModel.MainSets.SendEnableProcesses(this, () => gameContainer!.AlreadyDrew);
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    private void PossibleAutoResume()
    {
        if (gameContainer.TempSets.Count > 0)
        {
            var player = gameContainer.PlayerList!.GetSelf();
            bool hadAny = false;
            foreach (var item in gameContainer.TempSets)
            {
                var current = viewModel.TempSets.SetList[item.SetNumber - 1];
                var cards = item.Cards.GetNewObjectListFromDeckList(gameContainer.DeckList);
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
                viewModel.TempSets.PublicCount();
            }
        }
    }
    protected override bool CanEnableDeck()
    {
        return !gameContainer!.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    public bool CanSkipPlayer() => viewModel.PlayerChosen != "";
    [Command(EnumCommandCategory.Game)]
    public async Task SkipPlayerAsync()
    {
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("playerskipped", viewModel.PlayerChosen);
        }
        await mainGame.SkipPlayerAsync(viewModel.PlayerChosen);
    }
    public bool CanCompletePhase()
    {
        if (gameContainer!.AlreadyDrew == false)
        {
            return false;
        }
        return !mainGame.SaveRoot!.CompletedPhase;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task CompletePhaseAsync()
    {
        bool rets;
        rets = mainGame!.DidCompletePhase(out int Manys);
        if (Manys == viewModel.TempSets!.TotalObjects + mainGame.SingleInfo!.MainHandList.Count)
        {
            Toast.ShowUserErrorToast("Cannot complete the phase.  Otherwise; there is no card to discard");
            return;
        }
        if (rets == false)
        {
            Toast.ShowUserErrorToast("Sorry, you did not complete the phase");
            return;
        }
        var thisCol = mainGame.ListValidSets();
        if (thisCol.Count > 2)
        {
            throw new CustomBasicException("Can not have more than 2 sets");
        }
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (mainGame.BasicData!.MultiPlayer == true)
            {
                SendNewSet thisSend = new();
                thisSend.CardListData = await js1.SerializeObjectAsync(thisTemp.CardList.GetDeckListFromObjectList());
                thisSend.WhatSet = thisTemp.WhatSet;
                string newStr = await js1.SerializeObjectAsync(thisSend);
                newList.Add(newStr);
            }
            mainGame.CreateNewSet(thisTemp);
        });
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendSeveralSetsAsync(newList, "phasecompleted");
        }
        await mainGame.ProcessCompletedPhaseAsync();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        bool rets = mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message);
        if (rets == false)
        {
            if (message != "")
            {
                viewModel.PlayerHand1!.UnselectAllObjects();
                viewModel.TempSets!.UnselectAllCards(); //its best to just unselect everything if you can't discard.  that will solve some issues.
                Toast.ShowUserErrorToast(message); //because on tablets, its possible it can't show message
            }
            return;
        }
        if (pickUp == true)
        {
            await mainGame.PickupFromDiscardAsync();
            return;
        }
        await gameContainer.SendDiscardMessageAsync(deck);
        await mainGame.DiscardAsync(deck);
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
        var tempList = viewModel.PlayerHand1!.ListSelectedObjects(true);
        viewModel.TempSets!.AddCards(index, tempList);
        await privateAutoResume.SaveStateAsync(viewModel);
        _isProcessing = false;
    }
    private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            return;
        }
        if (mainGame!.SaveRoot!.CompletedPhase == false)
        {
            Toast.ShowUserErrorToast("Sorry, the phase must be completed before expanding onto a set");
            return;
        }
        var thisSet = viewModel.MainSets!.GetIndividualSet(setNumber);
        int position = section;
        bool rets;
        rets = mainGame.CanHumanExpand(thisSet, ref position, out Phase10CardInformation? thisCard, out string message);
        if (rets == false)
        {
            if (message != "")
            {
                Toast.ShowUserErrorToast(message);
            }
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            SendExpandedSet expands = new();
            expands.Deck = thisCard!.Deck;
            expands.Position = position;
            expands.Number = setNumber;
            await mainGame.Network!.SendAllAsync("expandrummy", expands);
        }
        await mainGame.ExpandHumanRummyAsync(setNumber, thisCard!.Deck, position);
    }
}