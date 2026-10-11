namespace CousinRummy.Core.Logic;
public class PhaseSet(CousinRummyGameContainer gameContainer) : SetInfo<EnumSuitList, EnumRegularColorList, RegularRummyCard, SavedSet>(gameContainer.Command)
{
    public override void LoadSet(SavedSet payLoad)
    {
        HandList.ReplaceRange(payLoad.CardList);
    }
    public override SavedSet SavedSet()
    {
        SavedSet output = new();
        output.CardList = HandList.ToRegularDeckDict();
        return output;
    }
    public void AddCard(RegularRummyCard thisCard)
    {
        UpdateCard(thisCard);
        HandList.Add(thisCard);
    }
    private void UpdateCard(RegularRummyCard thisCard)
    {
        thisCard.IsSelected = false;
        thisCard.Drew = false;
        thisCard.Player = gameContainer.WhoTurn;
    }
    public void CreateSet(IDeckDict<RegularRummyCard> thisList)
    {
        thisList.ForEach(thisCard => UpdateCard(thisCard));
        HandList.ReplaceRange(thisList);
    }
    public int PointsReceived(int player)
    {
        if (gameContainer.ModifyCards == null)
        {
            throw new CustomBasicException("Nobody is handling modify cards.  Rethink");
        }
        gameContainer.ModifyCards.Invoke(HandList);
        return HandList.Where(items => items.Player == player).Sum(items => items.Points);
    }
    public bool CanExpand(RegularRummyCard thisCard)
    {
        if (thisCard.IsObjectWild == true)
        {
            return true;
        }
        EnumRegularCardValueList numberNeeded = HandList.First(items => items.IsObjectWild == false).Value;
        return thisCard.Value == numberNeeded;
    }
}