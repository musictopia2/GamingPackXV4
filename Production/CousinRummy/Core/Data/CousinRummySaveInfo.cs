namespace CousinRummy.Core.Data;
[SingletonGame]
public class CousinRummySaveInfo : BasicSavedCardClass<CousinRummyPlayerItem, RegularRummyCard>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = [];
    public int Round { get; set; }
    public int WhoDiscarded { get; set; } //0 means nobody.
}