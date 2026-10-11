namespace Chinazo.Core.ViewModels;
[InstanceGame]
public partial class ChinazoMainViewModel(CommandContainer commandContainer,
        ChinazoMainGameClass mainGame,
        ChinazoVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ChinazoGameContainer gameContainer,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<ChinazoCard>(commandContainer ,mainGame, viewModel, basicData, resolver, aggregator, toast)
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
        viewModel.MainSets.SendEnableProcesses(this, () =>
        {
            if (mainGame!.OtherTurn > 0)
            {
                return false;
            }
            return mainGame.SingleInfo!.LaidDown;
        });
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
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
                viewModel.TempSets.PublicCount();
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
        return mainGame!.OtherTurn == 0;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int counts = viewModel.PlayerHand1!.HowManySelectedObjects;
        int others = viewModel.TempSets!.HowManySelectedObjects;
        if (counts + others > 1)
        {
            Toast.ShowUserErrorToast("Sorry, you can only select one card to discard");
            return;
        }
        if (counts + others == 0)
        {
            Toast.ShowUserErrorToast("Sorry, you must select a card to discard");
            return;
        }
        int index;
        int deck;
        if (counts == 0)
        {
            index = viewModel.TempSets.PileForSelectedObject;
            deck = viewModel.TempSets.DeckForSelectedObjected(index);
        }
        else
        {
            deck = viewModel.PlayerHand1.ObjectSelected();
        }
        await gameContainer!.SendDiscardMessageAsync(deck);
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
        var thisSet = viewModel.MainSets!.GetIndividualSet(setNumber);
        bool rets;
        rets = mainGame!.CanAddToSet(thisSet, out ChinazoCard? thisCard, section, out string message);
        if (rets == false)
        {
            if (message != "")
            {
                Toast.ShowUserErrorToast(message);
            }
            return;
        }
        int nums = setNumber;
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            SendExpandedSet thiss = new();
            thiss.Deck = thisCard!.Deck;
            thiss.Number = nums;
            thiss.Position = section;
            await mainGame.Network!.SendAllAsync("expandrummy", thiss);
        }
        await mainGame.AddToSetAsync(nums, thisCard!.Deck, section);
    }
    public bool CanPass => !CanEnablePile1();
    [Command(EnumCommandCategory.Game)]
    public async Task PassAsync()
    {
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("pass");
        }
        await mainGame!.PassAsync();
    }
    public bool CanTake => !CanEnablePile1();
    [Command(EnumCommandCategory.Game)]
    public async Task TakeAsync()
    {
        await mainGame!.PickupFromDiscardAsync();
    }
    public bool CanFirstSets
    {
        get
        {
            if (mainGame!.OtherTurn > 0)
            {
                return false;
            }
            return !mainGame.SingleInfo!.LaidDown;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task FirstSetsAsync()
    {
        if (mainGame!.CanLayDownInitialSets() == false)
        {
            Toast.ShowUserErrorToast("Sorry; you do not have the required sets yet");
            return;
        }
        var thisCol = mainGame.ListValidSets();
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (mainGame.BasicData!.MultiPlayer == true)
            {
                SendNewSet thiss = new();
                thiss.CardListData = await js1.SerializeObjectAsync(thisTemp.CardList.GetDeckListFromObjectList());
                thiss.UseSecond = thisTemp.UseSecond;
                thiss.WhatSet = thisTemp.WhatSet;
                var thisStr = await js1.SerializeObjectAsync(thiss);
                newList.Add(thisStr);
            }
            mainGame.CreateNewSet(thisTemp);
        });
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendSeveralSetsAsync(newList, "laiddowninitial");
        }
        await mainGame!.LaidDownInitialSetsAsync();
    }
}