namespace DutchBlitz.Core.Piles;
public class PublicViewModel(DutchBlitzGameContainer gameContainer) : PublicPilesVM<DutchBlitzCardInformation>(gameContainer.Command)
{
    protected override int MaximumAllowed => 10;
    public bool CanCreatePile(int deck)
    {
        var thisCard = gameContainer.DeckList!.GetSpecificItem(deck);
        return thisCard.CardValue == 1;
    }
    public bool CanAddToPile(int deck, int pile)
    {
        var thisCard = gameContainer.DeckList!.GetSpecificItem(deck);
        return CanAddToPile(thisCard, pile);
    }
    public bool CanAddToPile(DutchBlitzCardInformation newCard, int pile)
    {
        if (HasCard(pile + 1) == false)
        {
            return false;
        }
        if (gameContainer.Test!.AllowAnyMove == true && gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            return true; //while i test.
        }
        var thisCard = GetLastCard(pile + 1);
        return thisCard.CardValue + 1 == newCard.CardValue;
    }
    public int PointsPlayed(int player, DeckRegularDict<DutchBlitzCardInformation> otherList)
    {
        int output = 0;
        PileList.ForEach(thisPile =>
        {
            var tempList = thisPile.ObjectList.ToRegularDeckDict();
            output += tempList.Count(items => items.Player == player);
        });
        output += otherList.Count(items => items.Player == player);
        return output;
    }
}