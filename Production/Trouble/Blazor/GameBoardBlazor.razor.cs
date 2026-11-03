using System.Reflection;
namespace Trouble.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public TroubleGameContainer? GameContainer { get; set; }
    [Parameter]
    public double DiceHeight { get; set; } 
    [Parameter]
    public DiceCup<SimpleDice>? Cup { get; set; }
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; }
    [Parameter]
    public double TargetHeight { get; set; } 
    private string GetColor() => GetColor(GameContainer!.SingleInfo!);
    private static string GetColor(TroublePlayerItem player) => player.Color.Color;
    protected override bool ShouldRender()
    {
        return GameContainer!.SingleInfo!.Color != EnumColorChoice.None;
    }
    private static SizeF OriginalSize => GameBoardGraphicsCP.OriginalSize;
    private static float SpaceWidth => GameBoardGraphicsCP.SpaceSize.Width;
    private static PointF RecommendedPointForDice => GameBoardGraphicsCP.RecommendedPointForDice;
}