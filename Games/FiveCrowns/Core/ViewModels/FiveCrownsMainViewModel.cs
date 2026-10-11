namespace FiveCrowns.Core.ViewModels;
[InstanceGame]
public partial class FiveCrownsMainViewModel(CommandContainer commandContainer,
        FiveCrownsMainGameClass mainGame,
        FiveCrownsVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        FiveCrownsGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<FiveCrownsCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = false;
        var player = mainGame.PlayerList.GetSelf();
        player.DoInit();
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        viewModel.TempSets.Init(this);
        viewModel.TempSets.ClearBoard();
        PossibleAutoResume();
        viewModel.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        viewModel.MainSets.SendEnableProcesses(this, () => false);
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
                DeckRegularDict<FiveCrownsCardInformation> toAdd = [];
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
    [Command(EnumCommandCategory.OutOfTurn)]
    public void Back() //you can put back even if its not your turn.
    {
        var thisList = viewModel.TempSets!.ListSelectedObjects();
        thisList.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            viewModel.TempSets.RemoveObject(thisCard.Deck);
        });
        //no private save here now.
        FiveCrownsPlayerItem thisPlayer = mainGame!.PlayerList!.GetSelf();
        thisPlayer.MainHandList.AddRange(thisList);
        mainGame.SortCards();
    }
    public bool CanLayDownSets()
    {
        if (gameContainer!.AlreadyDrew == false)
        {
            return false;
        }
        return !mainGame.SaveRoot!.SetsCreated;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task LayDownSetsAsync()
    {
        if (mainGame!.SaveRoot!.PlayerWentOut > 0)
        {
            bool lats = mainGame.CanLaterLayDown();
            if (lats == false)
            {
                return;
            }
            var thisCol1 = mainGame.ListValidSets();
            await ProcessValidSetsAsync(thisCol1);
            return;
        }
        bool rets = mainGame.HasInitialSet();
        if (rets == false)
        {
            Toast.ShowUserErrorToast("Sorry, you do not have the valid sets needed to go out");
            return;
        }
        var thisCol2 = mainGame.ListValidSets();
        await ProcessValidSetsAsync(thisCol2);
    }
    protected override bool CanEnableDeck()
    {
        return !gameContainer!.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message) == false)
        {
            Toast.ShowUserErrorToast(message);
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
    private async Task ProcessValidSetsAsync(BasicList<TempInfo> thisCol)
    {
        BasicList<string> newList = [];
        await thisCol.ForEachAsync(async thisTemp =>
        {
            if (mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                var thisStr = await js1.SerializeObjectAsync(tempList);
                newList.Add(thisStr);
            }
            viewModel.TempSets!.ClearBoard(thisTemp.SetNumber);
            mainGame!.CreateSet(thisTemp.CardList);
        });
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendSeveralSetsAsync(newList, "finishedsets");
        }
        await mainGame!.FinishedSetsAsync();
    }
}