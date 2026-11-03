namespace Opetong.Core.Data;
[SingletonGame]
public class OpetongSaveInfo : BasicSavedCardClass<OpetongPlayerItem, RegularRummyCard>, IMappable, ISaveInfo
{
    public DeckRegularDict<RegularRummyCard> PoolList { get; set; } = [];
    public BasicList<SavedSet> SetList { get; set; } = [];
    public bool FirstTurn { get; set; }
    public int WhichPart { get; set; }
}