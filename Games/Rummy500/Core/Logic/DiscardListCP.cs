namespace Rummy500.Core.Logic;
public class DiscardListCP : HandObservable<RegularRummyCard>
{
    public DiscardListCP(CommandContainer command) : base(command)
    {
        AutoSelect = EnumHandAutoType.None;
        Text = "Discard";
    }
    public void AddToDiscard(RegularRummyCard thisCard)
    {
        thisCard.Drew = false;
        thisCard.IsSelected = false;
        HandList.Add(thisCard);
    }
    public void ClearDiscardList()
    {
        HandList.Clear();
    }
    public int LastCardDiscarded => HandList.Last().Deck;
    public DeckRegularDict<RegularRummyCard> DiscardListSelected(int deck)
    {
        if (deck == 0)
        {
            throw new CustomBasicException("Deck cannot be 0 for the discard selected.  Rethink");
        }
        DeckRegularDict<RegularRummyCard> output = new();
        bool didStart = false;
        HandList.ForEach(thisCard =>
        {
            if (thisCard.Deck == deck)
            {
                didStart = true;
            }
            if (didStart == true)
            {
                output.Add(thisCard);
            }
        });
        if (output.Count == 0)
        {
            throw new CustomBasicException("No cards for discard list.  Rethink");
        }
        return output;
    }
    public void RemoveFromPoint(int deck)
    {
        DeckRegularDict<RegularRummyCard> output = new();
        foreach (var thisCard in HandList)
        {
            if (thisCard.Deck == deck)
            {
                break;
            }
            output.Add(thisCard);
        }
        if (output.Count == HandList.Count)
        {
            throw new CustomBasicException("No starting point for removing cards from point");
        }
        HandList.ReplaceRange(output);
    }
}