namespace MonopolyCardGame.Core.Data;
public struct TempInfo
{
    public DeckRegularDict<MonopolyCardGameCardInformation> CardList { get; set; }
    public int SetNumber { get; set; }
}