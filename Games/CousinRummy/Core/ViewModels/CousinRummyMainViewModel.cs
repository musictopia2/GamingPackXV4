namespace CousinRummy.Core.ViewModels;
[InstanceGame]
public partial class CousinRummyMainViewModel(CommandContainer commandContainer,
        CousinRummyMainGameClass mainGame,
        CousinRummyVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        CousinRummyGameContainer gameContainer,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<RegularRummyCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = false;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        var player = mainGame.PlayerList.GetSelf();
        player.DoInit();
        viewModel.TempSets.Init(this);
        viewModel.TempSets.ClearBoard(); //try this too.
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
                viewModel.TempSets.PublicCount();
            }
        }
    }
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
        return;
    }
    private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            return;
        }
        var thisSet = viewModel.MainSets!.GetIndividualSet(setNumber);
        bool rets;
        rets = mainGame!.CanAddToSet(thisSet, out RegularRummyCard? thisCard, out string message);
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
            await mainGame.Network!.SendAllAsync("expandrummy", thiss);
        }
        await mainGame.AddToSetAsync(nums, thisCard!.Deck);
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
    public bool CanBuy => !CanEnablePile1();
    [Command(EnumCommandCategory.Game)]
    public async Task BuyAsync()
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
        var thisCol = mainGame.ListValidSets(true);
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                var thisStr = await js1.SerializeObjectAsync(tempList);
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
    public bool CanOtherSets
    {
        get
        {
            if (mainGame!.OtherTurn > 0)
            {
                return false;
            }
            return mainGame.SingleInfo!.LaidDown;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task OtherSetsAsync()
    {
        var thisCol = mainGame!.ListValidSets(false);
        if (thisCol.Count == 0)
        {
            Toast.ShowUserErrorToast("Sorry; you do not have any more sets to put down");
            return;
        }
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                var thisStr = await js1.SerializeObjectAsync(tempList);
                newList.Add(thisStr);
            }
            mainGame.CreateNewSet(thisTemp);
        });
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendSeveralSetsAsync(newList, "laydownothers");
        }
        await mainGame.LayDownOtherSetsAsync();
    }
}