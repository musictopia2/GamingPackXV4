namespace ClueBoardGame.Blazor;
public partial class RoomWeaponGraphics
{
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    
    [Parameter]
    public BasicList<WeaponInfo> Weapons { get; set; } = [];
    [Parameter]
    public RoomInfo? Room { get; set; }
    
}