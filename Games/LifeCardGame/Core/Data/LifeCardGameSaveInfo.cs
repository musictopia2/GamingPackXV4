namespace LifeCardGame.Core.Data;
[SingletonGame]
public class LifeCardGameSaveInfo : BasicSavedCardClass<LifeCardGamePlayerItem, LifeCardGameCardInformation>, IMappable, ISaveInfo
{
    public BasicList<int> YearList { get; set; } = new();
    public int YearsPassed() => YearList.Count * 10;
}