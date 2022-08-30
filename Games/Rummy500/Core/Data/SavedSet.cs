namespace Rummy500.Core.Data;
public class SavedSet
{
    public DeckRegularDict<RegularRummyCard> CardList { get; set; } = new();
    public EnumWhatSets SetType;
    public bool UseSecond;
}