using System.Reflection;
namespace Payday.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "";
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;
    [CascadingParameter]
    private TestOptions? Test { get; set; }
    private async Task SpaceClickedAsync(int space)
    {
        if (GraphicsData!.GameContainer.Command.IsExecuting || GraphicsData.GameContainer.SaveRoot!.GameStatus != EnumStatus.MakeMove)
        {
            return;
        }
        await GraphicsData.GameContainer.ProcessCustomCommandAsync(GraphicsData.GameContainer.SpaceClickedAsync!, space);
    }
}