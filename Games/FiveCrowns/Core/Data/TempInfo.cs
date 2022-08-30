namespace FiveCrowns.Core.Data;
public struct TempInfo
{
    public DeckRegularDict<FiveCrownsCardInformation> CardList;
    public int SetNumber { get; set; }
}