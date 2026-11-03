namespace TileRummy.Core.Data;
public class SavedSet
{
    public DeckRegularDict<TileInfo> TileList { get; set; } = [];
    public EnumWhatSets SetType { get; set; }
    public bool IsNew { get; set; }
}