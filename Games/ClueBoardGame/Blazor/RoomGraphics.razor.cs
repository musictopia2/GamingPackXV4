namespace ClueBoardGame.Blazor;
public partial class RoomGraphics
{
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    //protected override bool ShouldRender()
    //{
    //    if (GraphicsData is null)
    //    {
    //        return true;
    //    }
    //    return GraphicsData.CanRefreshBasicRoomsInfo(); //if you can't refresh weapons, then you can't refresh anything else.
    //}
}