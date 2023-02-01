namespace RollEm.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public GameBoardGraphicsCP? BoardData { get; set; }
    private BasicList<NumberInfo> _numbers = new();
    protected override void OnParametersSet()
    {
        _numbers = BoardData!.GetNumberList;
        base.OnParametersSet();
    }
    private async Task SpaceClickedAsync(NumberInfo number)
    {
        if (BoardData!.CanEnableMove == false)
        {
            return; //ignore because can't do.
        }
        await BoardData!.MakeMoveAsync(number);
    }
    private static string LineColor(NumberInfo number) => number.IsCrossed ? cc1.Red.ToWebColor() : cc1.LimeGreen.ToWebColor();
    private static string FrameText => GameBoardGraphicsCP.FrameText;
    private static SizeF OriginalSize => GameBoardGraphicsCP.OriginalSize;
}