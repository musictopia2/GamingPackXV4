using System.Reflection;
namespace ClueBoardGame.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;
    private async Task SpaceClickedAsync(int space)
    {
        await GraphicsData!.GameContainer!.ProcessCustomCommandAsync(GraphicsData.GameContainer.SpaceClickedAsync!, space);
    }
    private async Task RoomClickedAsync(int room)
    {
        await GraphicsData!.GameContainer!.ProcessCustomCommandAsync(GraphicsData.GameContainer.RoomClickedAsync!, room);
    }
}