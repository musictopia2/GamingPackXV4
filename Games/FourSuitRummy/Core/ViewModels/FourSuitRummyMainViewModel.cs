namespace FourSuitRummy.Core.ViewModels;
[InstanceGame]
public partial class FourSuitRummyMainViewModel(CommandContainer commandContainer,
        FourSuitRummyMainGameClass mainGame,
        FourSuitRummyVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        FourSuitRummyGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<RegularRummyCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        var player = mainGame.PlayerList.GetSelf();
        player.DoInit();
        viewModel.TempSets.Init(this);
        viewModel.TempSets.ClearBoard();
        PossibleAutoResume();
        viewModel.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        CreateCommands(CommandContainer);
        viewModel.Deck1.NeverAutoDisable = true;
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
    public bool CanPlaySets => gameContainer.AlreadyDrew;

    [Command(EnumCommandCategory.Game)]
    public async Task PlaySetsAsync()
    {
        BasicList<string> textList = [];
        var thisCol = mainGame!.SetList();
        mainGame.SingleInfo = mainGame.PlayerList!.GetWhoPlayer();
        if (thisCol.Count == 0)
        {
            return;
        }
        if (mainGame.Test!.DoubleCheck == true && thisCol.Count > 1)
        {
            throw new CustomBasicException("cannot have more than one for now for sets in beginning");
        }
        await thisCol.ForEachAsync(async thisInt =>
        {
            var temps = viewModel.TempSets.ObjectList(thisInt);
            var newCol = temps.ToRegularDeckDict();
            if (mainGame.SingleInfo.MainSets!.CanAddSet(temps))
            {
                if (mainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = newCol.GetDeckListFromObjectList();
                    var thisStr = await js1.SerializeObjectAsync(tempList);
                    textList.Add(thisStr);
                }
                viewModel.TempSets.ClearBoard(thisInt);
                mainGame.AddSet(newCol);
            }
        });
        if (gameContainer.Test.DoubleCheck == true && textList.Count > 1)
        {
            throw new CustomBasicException("cannot have more than one for now for sets for sending to players");
        }
        if (gameContainer.BasicData!.MultiPlayer == true && textList.Count > 0)
        {
            await gameContainer.Network!.SendSeveralSetsAsync(textList, "finishedsets");
        }
        await mainGame.ContinueTurnAsync();
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
}