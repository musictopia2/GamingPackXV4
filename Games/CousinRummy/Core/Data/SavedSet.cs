namespace CousinRummy.Core.Data;
public class SavedSet
{
    public DeckRegularDict<RegularRummyCard> CardList { get; set; } = new();
}