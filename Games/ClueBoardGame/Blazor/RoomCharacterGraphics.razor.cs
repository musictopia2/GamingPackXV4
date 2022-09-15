namespace ClueBoardGame.Blazor;
public partial class RoomCharacterGraphics
{
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    protected override bool ShouldRender()
    {
        if (GraphicsData is null)
        {
            return true;
        }
        return GraphicsData.CanRefreshCharactersRoomInfo(Room!);
    }
    [Parameter]
    public BasicList<CharacterInfo> Characters { get; set; } = new();
    [Parameter]
    public RoomInfo? Room { get; set; }
    [Parameter]
    public SizeF SpaceSize { get; set; }
}