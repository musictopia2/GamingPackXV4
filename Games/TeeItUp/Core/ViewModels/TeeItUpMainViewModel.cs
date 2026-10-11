namespace TeeItUp.Core.ViewModels;
[InstanceGame]
public class TeeItUpMainViewModel(CommandContainer commandContainer,
        TeeItUpMainGameClass mainGame,
        TeeItUpVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        TeeItUpGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<TeeItUpCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.OtherPile!.SendEnableProcesses(this, () => mainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning);
        LoadPlayerBoards();
        base.AdditionalInitialize();
    }
    private void LoadPlayerBoards()
    {
        mainGame.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.LoadPlayerBoard(gameContainer!);
            thisPlayer.PlayerBoard!.SendEnableProcesses(this!, () =>
            {
                if (mainGame.SaveRoot!.GameStatus != EnumStatusType.Beginning)
                {
                    return true;
                }
                return mainGame.WhoTurn == thisPlayer.Id;
            });
        });
    }
    protected override bool CanEnableDeck()
    {
        return mainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning && viewModel.OtherPile!.PileEmpty();
    }
    protected override bool CanEnablePile1()
    {
        return mainGame!.SaveRoot!.GameStatus != EnumStatusType.Beginning;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int oldDeck;
        if (viewModel.OtherPile!.PileEmpty() == true)
        {
            oldDeck = 0;
        }
        else
        {
            oldDeck = viewModel.OtherPile.GetCardInfo().Deck;
        }
        if (oldDeck == 0)
        {
            await mainGame!.PickupFromDiscardAsync();
            return;
        }
        if (oldDeck == gameContainer!.PreviousCard)
        {
            Toast.ShowUserErrorToast("Cannot discard the same card that was picked up");
            return;
        }
        TeeItUpPlayerItem tempItem;
        if (gameContainer.BasicData!.MultiPlayer == true)
        {
            tempItem = mainGame!.PlayerList!.GetSelf();
        }
        else
        {
            tempItem = mainGame!.PlayerList!.GetWhoPlayer();
        }
        try
        {
            int matches = tempItem.PlayerBoard!.ColumnMatched(oldDeck);
            if (matches > 0)
            {
                Toast.ShowUserErrorToast("Cannot discard a card because there is a match");
                return;
            }
        }
        catch (Exception ex)
        {
            throw new CustomBasicException($"Exception when trying to find out about column matching.  Message was {ex.Message}");
        }
        var thisCard = gameContainer.DeckList!.GetSpecificItem(oldDeck);
        if (thisCard.IsMulligan == true)
        {
            Toast.ShowUserErrorToast("Cannot discard a Mulligan");
            return;
        }
        if (mainGame.BasicData.MultiPlayer == true)
        {
            await gameContainer.SendDiscardMessageAsync(oldDeck);
        }
        await mainGame.DiscardAsync(oldDeck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}