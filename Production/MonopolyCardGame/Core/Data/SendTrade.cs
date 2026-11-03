namespace MonopolyCardGame.Core.Data;
public class SendTrade
{
    public BasicList<int> CardList { get; set; } = [];
    public int Player { get; set; }
}