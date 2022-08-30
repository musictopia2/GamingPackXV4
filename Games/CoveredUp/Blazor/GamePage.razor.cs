namespace CoveredUp.Blazor;
public partial class GamePage
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    [CascadingParameter]
    public IGameInfo? GameData { get; set; }
    [CascadingParameter]
    public BasicData? BasicData { get; set; }
    [CascadingParameter]
    public MultiplayerBasicParentShell? Shell { get; set; }
    [CascadingParameter]
    private MediaQueryListComponent? Media { get; set; }
    private int TargetHeight
    {
        get
        {
            if (Media!.DeviceCategory == EnumDeviceCategory.Phone)
            {
                return 13;
            }
            return 16;
        }
    }
}