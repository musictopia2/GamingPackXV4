namespace TileRummy.Core.Data;
[SingletonGame]
public class TileRummySaveInfo : BasicSavedGameClass<TileRummyPlayerItem>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = new();
    public BasicList<SavedSet> BeginningList { get; set; } = new(); //this is at the beginning.
    public SavedScatteringPieces<TileInfo>? PoolData { get; set; }
    public int FirstPlayedLast { get; set; }
    public BasicList<int> TilesFromField { get; set; } = new();
    public BasicList<int> YourTiles { get; set; } = new();
    public bool DidExpand { get; set; }
}