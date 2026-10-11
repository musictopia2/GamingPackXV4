namespace ThreeLetterFun.Core.Data;
[SingletonGame]
public class ThreeLetterFunSaveInfo : BasicSavedGameClass<ThreeLetterFunPlayerItem>, IMappable, ISaveInfo
{
    public EnumLevel Level { get; set; }
    public BasicList<TileInformation> TileList { get; set; } = [];
    public DeckRegularDict<ThreeLetterFunCardData> SavedList { get; set; } = [];
    public bool ShortGame { get; set; }
    public int UpTo { get; set; }
    public int CardsToBeginWith { get; set; }
    public bool CanStart { get; set; }
}