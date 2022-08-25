namespace Poker.Core.Data;
[SingletonGame]
public class PokerSaveInfo : IMappable, ISaveInfo
{
    public BasicList<int> DeckList { get; set; } = new();
}