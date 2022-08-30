namespace FiveCrowns.Core.Data;
public class SavedSet
{
    public DeckRegularDict<FiveCrownsCardInformation> CardList { get; set; } = new();
}