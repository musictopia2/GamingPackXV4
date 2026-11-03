namespace Opetong.Core.ViewModels;
[InstanceGame]
public partial class OpetongMainViewModel(CommandContainer commandContainer,
        OpetongMainGameClass mainGame,
        OpetongVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume,
        OpetongGameContainer gameContainer) : BasicCardGamesVM<RegularRummyCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        var player = mainGame.PlayerList.GetSelf();
        player.DoInit();
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.TempSets.Init(this);
        viewModel.TempSets.ClearBoard();
        PossibleAutoResume();
        viewModel.MainSets.SendEnableProcesses(this, () => false);
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        viewModel.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
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
    protected override bool CanEnableDeck()
    {
        return true;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override Task ProcessDiscardClickedAsync()
    {
        throw new CustomBasicException("Discard should never be used this time.");
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PlaySetAsync()
    {
        int nums = mainGame!.FindValidSet();
        if (nums == 0)
        {
            Toast.ShowUserErrorToast("Sorry, there is no valid set here");
            return;
        }
        var thisCol = viewModel.TempSets.ObjectList(nums).ToRegularDeckDict();
        viewModel.TempSets.ClearBoard(nums);
        if (mainGame.BasicData!.MultiPlayer)
        {
            var tempCol = thisCol.GetDeckListFromObjectList();
            await mainGame.Network!.SendAllAsync("newset", tempCol);
        }
        await mainGame.PlaySetAsync(thisCol);
    }
}