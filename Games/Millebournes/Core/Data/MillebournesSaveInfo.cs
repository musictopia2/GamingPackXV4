namespace Millebournes.Core.Data;
[SingletonGame]
public class MillebournesSaveInfo : BasicSavedCardClass<MillebournesPlayerItem, MillebournesCardInformation>, IMappable, ISaveInfo
{
    public int LastThrowAway { get; set; }
    public int CurrentTeam { get; set; }
    public bool DidClone100 { get; set; }
    public BasicList<TempData> TeamData { get; set; } = new();
}