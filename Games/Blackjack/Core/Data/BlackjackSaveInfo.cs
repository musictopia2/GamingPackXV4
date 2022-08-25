namespace Blackjack.Core.Data;
[SingletonGame]
public class BlackjackSaveInfo : IMappable, ISaveInfo
{
    public BasicList<int> DeckList { get; set; } = new();
}