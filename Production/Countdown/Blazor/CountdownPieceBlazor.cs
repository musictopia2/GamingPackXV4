namespace Countdown.Blazor;
public class CountdownPieceBlazor : NumberPiece
{
    [Parameter]
    public SimpleNumber? Number { get; set; }
    [Parameter]
    public bool IsEnabled { get; set; }
    protected override void OriginalSizeProcesses()
    {
        MainGraphics!.OriginalSize = new SizeF(60, 40);
    }
    protected override bool ShouldRender()
    {
        return true; //this time, just set to true.  hopefully does not cause performance problems.  the risk i have to take this time.
    }
    protected override void OnParametersSet()
    {
        MainGraphics!.CustomCanDo = () => IsEnabled;
        if (CountdownVMData.ShowHints == false || CountdownVMData.CanChooseNumber!(Number!) == false)
        {
            MainGraphics!.FillColor = cs1.Aqua;
            return;
        }
        MainGraphics!.FillColor = cs1.Yellow;
    }
    protected override bool CanDrawNumber()
    {
        return !Number!.Used;
    }
    protected override string GetValueToPrint()
    {
        return Number!.Value.ToString();
    }
}