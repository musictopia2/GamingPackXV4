namespace MilkRun.Core.Data;
[SingletonGame]
public class MilkRunSaveInfo : BasicSavedCardClass<MilkRunPlayerItem, MilkRunCardInformation>, IMappable, ISaveInfo
{
    public int CardsDrawn { get; set; }
    public bool DrawnFromDiscard { get; set; }
}