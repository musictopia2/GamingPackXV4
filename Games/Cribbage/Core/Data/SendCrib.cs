namespace Cribbage.Core.Data;
public class SendCrib
{
    public DeckRegularDict<CribbageCard> CardList { get; set; } = new();
}