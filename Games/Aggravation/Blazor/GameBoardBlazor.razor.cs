using System.Reflection;
namespace Aggravation.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    [Parameter]
    public AggravationGameContainer? GameContainer { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "";
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;
    private string GetColor() => GetColor(GameContainer!.SingleInfo!);
    private static string GetColor(AggravationPlayerItem player) => player.Color.Color;
    protected override bool ShouldRender()
    {
        return GameContainer!.SingleInfo!.Color != EnumColorChoice.None;
    }
    private static float SpaceWidth => GameBoardGraphicsCP.SpaceSize.Width;
    private static SizeF OriginalSize => GameBoardGraphicsCP.OriginalSize;
}