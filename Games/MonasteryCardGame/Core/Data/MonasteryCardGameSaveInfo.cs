namespace MonasteryCardGame.Core.Data;
[SingletonGame]
public class MonasteryCardGameSaveInfo : BasicSavedCardClass<MonasteryCardGamePlayerItem, MonasteryCardInfo>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = new();
    public int Mission { get; set; }
}