namespace ClueBoardGame.Blazor;
public partial class DetectiveGraphicsBlazor
{
    [CascadingParameter]
    public EnumDetectiveCategory DetectiveCategory { get; set; }
    [CascadingParameter]
    public ClueBoardGameMainViewModel? DataContext { get; set; }
    [Parameter]
    public Dictionary<int, DetectiveInfo>? DetectiveList { get; set; }
    private BasicList<DetectiveInfo> _roomList = new();
    private BasicList<DetectiveInfo> _weaponList = new();
    private BasicList<DetectiveInfo> _characterList = new();
    protected override void OnParametersSet()
    {
        if (DetectiveList == null)
        {
            return;
        }
        _roomList = DetectiveList.Values.Where(x => x.Category == EnumCardType.IsRoom).ToBasicList();
        _weaponList = DetectiveList.Values.Where(x => x.Category == EnumCardType.IsWeapon).ToBasicList();
        _characterList = DetectiveList.Values.Where(x => x.Category == EnumCardType.IsCharacter).ToBasicList();
        base.OnParametersSet();
    }
    private static int StartX(EnumCardType category)
    {
        return category switch
        {
            EnumCardType.IsRoom => 0,
            EnumCardType.IsWeapon => pg.RoomWidth + pg.CharacterWidth,
            EnumCardType.IsCharacter => pg.RoomWidth,
            _ => -10,
        };
    }
    private static int ColumnWidth(EnumCardType category)
    {
        return category switch
        {
            EnumCardType.IsRoom => pg.RoomWidth,
            EnumCardType.IsWeapon => pg.RoomWidth,
            EnumCardType.IsCharacter => pg.CharacterWidth,
            _ => 10
        };
    }
    private static RectangleF GetButtonLocation(EnumCardType category, int row) //sent 0 based.
    {
        int startY = pg.CellHeight * row;
        int startX = StartX(category);
        int width = ColumnWidth(category);
        return new RectangleF(startX, startY, width, pg.CellHeight);
    }
    private int GetIndex(DetectiveInfo detective)
    {
        return detective.Category switch
        {
            EnumCardType.IsRoom => _roomList.IndexOf(detective),
            EnumCardType.IsWeapon => _weaponList.IndexOf(detective),
            EnumCardType.IsCharacter => _characterList.IndexOf(detective),
            _ => -1
        };
    }
    private RectangleF GetButtonLocation(DetectiveInfo detective)
    {
        int row = GetIndex(detective);
        return GetButtonLocation(detective.Category, row);
    }
}