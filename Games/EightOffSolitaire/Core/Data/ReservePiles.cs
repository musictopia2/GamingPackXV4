namespace EightOffSolitaire.Core.Data;
public class ReservePiles : HandObservable<SolitaireCard>
{
    public ReservePiles(CommandContainer command) : base(command)
    {
    }
    public int HowManyCards => HandList.Count;
    public void AddCard(SolitaireCard thisCard) => HandList.Add(thisCard);
    public void ClearBoard() => HandList.Clear();
    public void RemoveCard(SolitaireCard thisCard) => HandList.RemoveObjectByDeck(thisCard.Deck);
    public SolitaireCard GetCardSelected()
    {
        int nums = ObjectSelected();
        if (nums == 0)
        {
            return new SolitaireCard();
        }
        return HandList.GetSpecificItem(nums);
    }
}