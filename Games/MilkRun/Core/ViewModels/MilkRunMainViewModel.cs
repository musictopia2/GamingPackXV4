namespace MilkRun.Core.ViewModels;
[InstanceGame]
public class MilkRunMainViewModel(CommandContainer commandContainer,
        MilkRunMainGameClass mainGame,
        MilkRunVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        MilkRunGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<MilkRunCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.PlayerHand1.Maximum = 8;
        base.AdditionalInitialize();
    }
    protected override bool CanEnableDeck()
    {
        if (viewModel.Deck1.IsEndOfDeck())
        {
            return false; //because its the end of the deck period now.
        }
        return mainGame!.SaveRoot!.CardsDrawn != 2;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int newDeck = viewModel.PlayerHand1!.ObjectSelected();
        if (newDeck > 0)
        {
            if (mainGame!.SaveRoot!.CardsDrawn < 2 && viewModel.Deck1.IsEndOfDeck() == false)
            {
                Toast.ShowUserErrorToast("Sorry, must draw the 2 cards first before discarding");
                return;
            }
            if (newDeck == gameContainer.PreviousCard)
            {
                Toast.ShowUserErrorToast("Cannot discard the same card you picked up");
                return;
            }
            await gameContainer.SendDiscardMessageAsync(newDeck);
            await mainGame.DiscardAsync(newDeck);
            return;
        }
        if (mainGame.SaveRoot.CardsDrawn == 2)
        {
            Toast.ShowUserErrorToast("You already drew 2 cards.  Therefore, cannot pick up from discard");
            return;
        }
        if (mainGame!.SaveRoot!.DrawnFromDiscard == true)
        {
            Toast.ShowUserErrorToast("Sorry, you already picked up one card from discard.  Cannot pickup another one.  If you want to discard, then choose a card to discard");
            return;
        }
        if (viewModel.Pile1!.PileEmpty())
        {
            Toast.ShowUserErrorToast("Sorry, there is no card to pickup from the discard.");
            return;
        }
        await mainGame.PickupFromDiscardAsync();
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}