namespace Rummy500.Core.Data;
public class SavedSet
{
    public DeckRegularDict<RegularRummyCard> CardList { get; set; } = [];
    public EnumWhatSets SetType { get; set; }
    public bool UseSecond { get; set; }
}