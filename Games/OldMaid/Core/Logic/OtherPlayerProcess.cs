namespace OldMaid.Core.Logic;
[SingletonGame]
[AutoReset]
public class OtherPlayerProcess(OldMaidGameContainer gameContainer, OldMaidVMData model) : IOtherPlayerProcess
{
    public void SortOtherCards()
    {
        DeckRegularDict<RegularSimpleCard> output = [];
        gameContainer!.OtherPlayer!.MainHandList.ForEach(thisCard =>
        {
            RegularSimpleCard newCard = new();
            newCard.Populate(thisCard.Deck);
            newCard.IsUnknown = true;
            output.Add(newCard);
        });
        output.ShuffleList();
        model.OpponentCards1!.HandList.ReplaceRange(output);
    }
    async Task IOtherPlayerProcess.SelectCardAsync(int deck)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!) == true)
        {
            await gameContainer.Network!.SendAllAsync("cardselected", deck);
        }
        if (gameContainer!.OtherPlayer!.PlayerCategory == EnumPlayerCategory.Self)
        {
            model.PlayerHand1!.SelectOneFromDeck(deck);
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(.75);
            }
        }
        gameContainer.OtherPlayer.MainHandList.RemoveObjectByDeck(deck);
        var thisCard = gameContainer.DeckList!.GetSpecificItem(deck);
        thisCard.IsUnknown = false;
        thisCard.IsSelected = false;
        SortOtherCards();
        gameContainer.SingleInfo!.MainHandList.Add(thisCard);
        if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            thisCard.Drew = true;
            if (gameContainer.SortCards == null)
            {
                throw new CustomBasicException("Nobody is handling sort cards.  Rethink");
            }
            gameContainer.SortCards.Invoke();
        }
        gameContainer.SaveRoot!.AlreadyChoseOne = true;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}