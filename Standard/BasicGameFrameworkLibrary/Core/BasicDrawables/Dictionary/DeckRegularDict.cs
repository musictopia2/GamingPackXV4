namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Dictionary;
public class DeckRegularDict<D> : BasicList<D>, IDeckDict<D>, IEnumerableDeck<D> where D : IDeckObject
{
    private DictionaryBehavior<D>? _thisB;
    public DeckRegularDict() : base() { }
    public DeckRegularDict(IEnumerable<D> thisList) : base(thisList) { }
    protected override void LoadBehavior()
    {
        _thisB = new DictionaryBehavior<D>();
        Behavior = _thisB;
    }
    public D GetSpecificItem(int deck)
    {
        return _thisB!.SearchItem(deck);
    }
    public bool ObjectExist(int deck)
    {
        return _thisB!.ObjectExist(deck);
    }
    public D RemoveObjectByDeck(int deck)
    {
        D thisCard = GetSpecificItem(deck);
        RemoveSpecificItem(thisCard);
        return thisCard;
    }
    public void ReplaceDictionary(int oldValue, int deck, D newValue)
    {
        _thisB!.ReplaceDictionaryValue(oldValue, deck, newValue);
    }
}