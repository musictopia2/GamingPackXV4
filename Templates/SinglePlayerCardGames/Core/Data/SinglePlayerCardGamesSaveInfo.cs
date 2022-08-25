namespace SinglePlayerCardGames.Core.Data;
[SingletonGame]
public class SinglePlayerCardGamesSaveInfo : IMappable, ISaveInfo
{
    public BasicList<int> DeckList { get; set; } = new();
}