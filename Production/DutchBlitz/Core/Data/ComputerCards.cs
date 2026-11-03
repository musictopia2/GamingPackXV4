namespace DutchBlitz.Core.Data;
public class ComputerCards
{
    public DeckRegularDict<DutchBlitzCardInformation> StockList = [];
    public DeckRegularDict<DutchBlitzCardInformation> DeckList = [];
    public DeckRegularDict<DutchBlitzCardInformation> Discard = [];
    public int Player { get; set; }
    public DeckRegularDict<DutchBlitzCardInformation> PileList = [];
}