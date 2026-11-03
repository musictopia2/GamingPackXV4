namespace Rummy500.Core.ViewModels;
[InstanceGame]
public partial class Rummy500MainViewModel(CommandContainer commandContainer,
        Rummy500MainGameClass mainGame,
        Rummy500VMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        Rummy500GameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularRummyCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)    
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        viewModel.MainSets1.SetClickedAsync = MainSets1_SetClickedAsync;
        viewModel.DiscardList1.ObjectClickedAsync = DiscardList1_ObjectClickedAsync;
        viewModel.DiscardList1.BoardClickedAsync = DiscardList1_BoardClickedAsync;
        viewModel.MainSets1.SendEnableProcesses(this, () => gameContainer!.AlreadyDrew);
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        return !gameContainer.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    private bool _didClickDiscard;
    private async Task DiscardList1_BoardClickedAsync()
    {
        if (_didClickDiscard == true)
        {
            _didClickDiscard = false;
            return;
        }
        _didClickDiscard = true;
        await NewDiscardClickAsync(0);
    }
    private async Task DiscardList1_ObjectClickedAsync(RegularRummyCard thisObject, int index)
    {
        _didClickDiscard = true;
        await NewDiscardClickAsync(thisObject.Deck);
    }
    private async Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
    {
        var newcol = viewModel.PlayerHand1!.ListSelectedObjects();
        if (newcol.Count == 0)
        {
            Toast.ShowUserErrorToast("There is no card selected");
            return;
        }
        if (newcol.Count > 1)
        {
            Toast.ShowUserErrorToast("Only can expand one card at a time");
            return;
        }
        if (viewModel.PlayerHand1.HandList.Count == 1)
        {
            Toast.ShowUserErrorToast("Sorry, must have a card left for discard");
            return;
        }
        if (mainGame.NeedsExpansion(newcol))
        {
            Toast.ShowUserErrorToast("Needs to use the extra card picked up first before anything else");
            return;
        }
        var thisCard = newcol.First();
        RummySet thisSet = viewModel.MainSets1!.GetIndividualSet(setNumber);
        int pos = thisSet.PositionToPlay(thisCard);
        if (pos == 0)
        {
            Toast.ShowUserErrorToast("This cannot be used to expand upto");
            return;
        }
        var thisCol = viewModel.MainSets1.SetList.ToBasicList();
        int x = 0;
        int nums = 0;
        thisCol.ForEach(newSet =>
        {
            x++;
            if (newSet.Equals(thisSet))
            {
                nums = x;
            }
        });
        if (nums == 0)
        {
            throw new CustomBasicException("Cannot find the rummy set that matches");
        }
        if (gameContainer.BasicData!.MultiPlayer == true)
        {
            SendAddSet thisSend = new();
            thisSend.Deck = thisCard.Deck;
            thisSend.Position = pos;
            thisSend.Index = nums;
            await gameContainer.Network!.SendAllAsync("addtoset", thisSend);
        }
        await mainGame!.AddToSetAsync(nums, thisCard.Deck, pos);
    }
    private async Task NewDiscardClickAsync(int deck)
    {
        if (mainGame!.CanProcessDiscard(out bool pickUp, ref deck, out string message) == false)
        {
            Toast.ShowUserErrorToast(message);
            return;
        }
        if (pickUp == true)
        {
            var thisCol = viewModel.DiscardList1!.DiscardListSelected(deck);
            if (thisCol.Count > 1)
            {
                var newCol = mainGame.AppendDiscardList(thisCol);
                if (mainGame.CardContainsRummy(deck, newCol) == false)
                {
                    Toast.ShowUserErrorToast("Sorry, cannot pick up more than one card because either invalid rummy or no card left for discard");
                    return;
                }
            }
            if (gameContainer.BasicData!.MultiPlayer == true)
            {
                await gameContainer.Network!.SendAllAsync("pickupfromdiscard", deck);
            }
            await mainGame.PickupFromDiscardAsync(deck);
            return;
        }
        await gameContainer.SendDiscardMessageAsync(deck);
        await mainGame.DiscardAsync(deck);
    }
    public bool CanCreateSet => gameContainer.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task CreateSetAsync()
    {
        var thisCol = viewModel.PlayerHand1.ListSelectedObjects();
        if (thisCol.Count == viewModel.PlayerHand1.HandList.Count)
        {
            Toast.ShowUserErrorToast("Sorry, you must have one card left over to discard");
            viewModel.PlayerHand1.UnselectAllObjects();
            return;
        }
        if (mainGame.NeedsExpansion(thisCol))
        {
            Toast.ShowUserErrorToast("Needs to use the extra card picked up first before anything else");
            return;
        }
        if (mainGame!.IsValidRummy(thisCol, out EnumWhatSets settype, out bool seconds) == false)
        {
            Toast.ShowUserErrorToast("This is not a valid rummy");
            viewModel.PlayerHand1.UnselectAllObjects();
            return;
        }
        if (thisCol.Count == 1)
        {
            if (thisCol.Single().Deck == gameContainer.PreviousCard)
            {
                Toast.ShowUserErrorToast("Sorry, since the last card left is the card picked up, then cannot put down the rummy");
                return;
            }
        }
        if (gameContainer.BasicData!.MultiPlayer == true)
        {
            SendNewSet thisNew = new();
            thisNew.DeckList = thisCol.GetDeckListFromObjectList();
            thisNew.SetType = settype;
            thisNew.UseSecond = seconds;
            await gameContainer.Network!.SendAllAsync("newset", thisNew);
        }
        await mainGame.CreateNewSetAsync(thisCol, settype, seconds);
    }
    public bool CanDiscardCurrent => gameContainer.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task DiscardCurrentAsync()
    {
        await NewDiscardClickAsync(0);
    }
}