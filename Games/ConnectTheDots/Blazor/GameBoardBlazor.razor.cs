namespace ConnectTheDots.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public GameBoardGraphicsCP? Graphics { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public ConnectTheDotsGameContainer? Container { get; set; }
    private string LineColor(LineInfo line)
    {
        if (line.Equals(Container!.PreviousLine))
        {
            return cc1.Green.ToWebColor;
        }
        return cc1.Brown.ToWebColor;
    }
    private static string SquareColor(SquareInfo square)
    {
        if (square.Color == 1)
        {
            return cc1.Blue.ToWebColor;
        }
        return cc1.Red.ToWebColor;
    }
    private async Task MakeMoveAsync(int index)
    {
        if (Container!.MakeMoveAsync == null)
        {
            return;
        }
        await Container.MakeMoveAsync(index);
    }
}
