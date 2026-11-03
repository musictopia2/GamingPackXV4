namespace Pinochle2Player.Core.ViewModels;
[InstanceGame]
public partial class Pinochle2PlayerMainViewModel(CommandContainer commandContainer,
        Pinochle2PlayerMainGameClass mainGame,
        Pinochle2PlayerVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<Pinochle2PlayerCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.OpponentMelds.SendEnableProcesses(this, () => false);
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        viewModel.YourMelds.SendEnableProcesses(this, () =>
        {
            if (mainGame!.SaveRoot!.ChooseToMeld)
            {
                viewModel.YourMelds.AutoSelect = EnumHandAutoType.SelectAsMany;
                return true;
            }
            viewModel.YourMelds.AutoSelect = EnumHandAutoType.SelectOneOnly;
            return viewModel.Pile1!.PileEmpty() == false && mainGame.SaveRoot.MeldList.Any(items => items.Player == mainGame.WhoTurn && items.CardList.Count > 0);
        });
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        CommandContainer.ExecutingChanged = null;
        return base.CloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return; //for now, has to be this way.
        }
        if (mainGame!.SaveRoot!.ChooseToMeld)
        {
            viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        else
        {
            viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        if (mainGame!.SaveRoot!.ChooseToMeld == false)
        {
            return false;
        }
        return viewModel.Pile1!.GetCardInfo().Value != EnumRegularCardValueList.Nine && mainGame.SingleInfo!.MainHandList.Any(items => items.Value == EnumRegularCardValueList.Nine && items.Suit == mainGame.SaveRoot.TrumpSuit);
    }
    public override bool CanEndTurn()
    {
        if (viewModel.PlayerHand1!.HasSelectedObject() || viewModel.YourMelds!.HasSelectedObject())
        {
            return false;
        }
        return mainGame!.SaveRoot!.ChooseToMeld;
    }

    protected override async Task ProcessDiscardClickedAsync()
    {
        int hands = viewModel.PlayerHand1!.HowManySelectedObjects;
        if (hands == 0)
        {
            Toast.ShowUserErrorToast("Must choose a card from hand in order to exchange for the top card");
            return;
        }
        if (hands > 1)
        {
            Toast.ShowUserErrorToast("Must choose only one card from hand to exchange");
            return;
        }
        Pinochle2PlayerCardInformation thisCard;
        thisCard = mainGame!.SingleInfo!.MainHandList.GetSpecificItem(viewModel.PlayerHand1.ObjectSelected());
        if (thisCard.Value > EnumRegularCardValueList.Nine)
        {
            Toast.ShowUserErrorToast("Must choose a nine to exchange");
            return;
        }
        if (thisCard.Suit != mainGame!.SaveRoot!.TrumpSuit)
        {
            Toast.ShowUserErrorToast("Must choose the nine of the trump suit in order to exchange");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("exchangediscard", thisCard.Deck);
        }
        await mainGame.ExchangeDiscardAsync(thisCard.Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanMeld => mainGame!.SaveRoot!.ChooseToMeld;
    [Command(EnumCommandCategory.Game)]
    public async Task MeldAsync()
    {
        if (viewModel.PlayerHand1!.HasSelectedObject() == false)
        {
            Toast.ShowUserErrorToast("Must choose at least one card from hand in order to meld");
            return;
        }
        var completeList = viewModel.PlayerHand1.ListSelectedObjects();
        DeckRegularDict<Pinochle2PlayerCardInformation> otherList = [];
        if (viewModel.YourMelds!.HasSelectedObject())
        {
            otherList = viewModel.YourMelds.ListSelectedObjects();
            completeList.AddRange(otherList);
        }
        var thisMeld = mainGame!.GetMeldFromList(completeList);
        if (thisMeld.ClassAValue == EnumClassA.None && thisMeld.ClassBValue == EnumClassB.None && thisMeld.ClassCValue == EnumClassC.None)
        {
            Toast.ShowUserErrorToast("There is no meld combinations here");
            return;
        }
        if (viewModel.YourMelds.HasSelectedObject())
        {
            foreach (var thisCard in otherList)
            {
                var tempMeld = mainGame.GetMeldFromCard(thisCard);
                if (tempMeld.ClassAValue == EnumClassA.Dix)
                {
                    throw new CustomBasicException("Should have caught the problem with using dix earlier");
                }
                if (tempMeld.ClassBValue == thisMeld.ClassBValue && thisMeld.ClassBValue > EnumClassB.None)
                {
                    Toast.ShowUserErrorToast("Cannot reuse class b for class b");
                    return;
                }
                if (tempMeld.ClassCValue == thisMeld.ClassCValue && thisMeld.ClassCValue > EnumClassC.None)
                {
                    Toast.ShowUserErrorToast("Cannot reuse a pinochle for a pinochle");
                    return;
                }
                if (tempMeld.ClassAValue <= thisMeld.ClassAValue && thisMeld.ClassAValue > EnumClassA.None)
                {
                    Toast.ShowUserErrorToast("Cannot download class A to get more points or to create another of same value");
                    return;
                }
            }
        }
        var deckList = completeList.GetDeckListFromObjectList();
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("meld", deckList);
        }
        await mainGame.MeldAsync(deckList);
    }
}