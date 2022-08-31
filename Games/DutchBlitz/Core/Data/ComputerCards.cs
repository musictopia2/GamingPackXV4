namespace DutchBlitz.Core.Data;
public class ComputerCards
{
    public DeckRegularDict<DutchBlitzCardInformation> StockList = new();
    public DeckRegularDict<DutchBlitzCardInformation> DeckList = new();
    public DeckRegularDict<DutchBlitzCardInformation> Discard = new();
    public int Player { get; set; }
    public DeckRegularDict<DutchBlitzCardInformation> PileList = new();
}