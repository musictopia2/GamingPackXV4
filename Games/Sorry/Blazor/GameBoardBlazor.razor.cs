using System.Reflection;
namespace Sorry.Blazor;
public partial class GameBoardBlazor
{
    private bool CanRenderBeginnings()
    {
        if (GraphicsData!.Container.Animates.AnimationGoing)
        {
            return false; //if animation is going, should not even consider this
        }
        if (GraphicsData.Container.MovePlayer > 0)
        {
            return false;
        }
        return true;
    }
    [Parameter]
    public GameBoardGraphicsCP? GraphicsData { get; set; } //this includes the container that is needed as well.
    [Parameter]
    public string TargetHeight { get; set; } = "";
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;
    private static string GetColor(SorryPlayerItem player) => player.Color.Color;
    private async Task SpaceClicked(int space)
    {
        if (GraphicsData!.Container.SpaceClickedAsync == null)
        {
            return;
        }
        if (GraphicsData.Container.SaveRoot.DidDraw == false)
        {
            return; //because you have to draw first.
        }
        await GraphicsData.Container.SpaceClickedAsync(space);
    }
    //only can click for whosever turn it is.
    private async Task HomeClicked()
    {
        if (GraphicsData!.Container.HomeClickedAsync == null)
        {
            GraphicsData.Container.Command.StopExecuting();
            return;
        }
        if (GraphicsData.Container.SaveRoot.DidDraw == false)
        {
            GraphicsData.Container.Command.StopExecuting();
            return; //because you have to draw first.
        }
        await GraphicsData.Container.HomeClickedAsync(GraphicsData.Container.SingleInfo!.Color);
    }
    private async Task DrawClicked()
    {
        if (GraphicsData!.Container.DrawClickAsync == null)
        {
            GraphicsData.Container.Command.StopExecuting();
            return;
        }
        if (GraphicsData.Container.SaveRoot.DidDraw == true)
        {
            GraphicsData.Container.Command.StopExecuting();
            return; //because you already drew.
        }
        await GraphicsData.Container.DrawClickAsync();
    }
    protected override bool ShouldRender()
    {
        return GraphicsData!.Container.SingleInfo!.Color != EnumColorChoice.None;
    }
}