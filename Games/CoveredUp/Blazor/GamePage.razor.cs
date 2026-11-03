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
    private static int TargetHeight
    {
        get
        {
            if (ll1.LayoutMode == EnumGameLayoutMode.Compact)
            {
                return 18;
            }

            return 22;

            
        }
    }
}