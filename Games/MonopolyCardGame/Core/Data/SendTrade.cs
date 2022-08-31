namespace MonopolyCardGame.Core.Data;
public class SendTrade
{
    public BasicList<int> CardList { get; set; } = new();
    public int Player { get; set; }
}