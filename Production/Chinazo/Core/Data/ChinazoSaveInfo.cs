namespace Chinazo.Core.Data;
[SingletonGame]
public class ChinazoSaveInfo : BasicSavedCardClass<ChinazoPlayerItem, ChinazoCard>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = [];
    public int Round { get; set; }
    public bool HadChinazo { get; set; }
}