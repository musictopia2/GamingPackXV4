namespace TileRummy.Core.Data;
[SingletonGame]
public class TileRummySaveInfo : BasicSavedGameClass<TileRummyPlayerItem>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = [];
    public BasicList<SavedSet> BeginningList { get; set; } = []; //this is at the beginning.
    public SavedScatteringPieces<TileInfo>? PoolData { get; set; }
    public int FirstPlayedLast { get; set; }
    public BasicList<int> TilesFromField { get; set; } = [];
    public BasicList<int> YourTiles { get; set; } = [];
    public bool DidExpand { get; set; }
    public bool FirstInit { get; set; }
}