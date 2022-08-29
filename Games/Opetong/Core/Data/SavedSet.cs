namespace Opetong.Core.Data;
public class SavedSet
{
    public int Player { get; set; }
    public DeckRegularDict<RegularRummyCard> CardList { get; set; } = new();
}