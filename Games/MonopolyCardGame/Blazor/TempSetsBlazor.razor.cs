namespace MonopolyCardGame.Blazor;
public partial class TempSetsBlazor
{
    [Parameter]
    public TempSets? TempPiles { get; set; }
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public double AdditionalSpacing { get; set; } = -5;
    [Parameter]
    public string TargetContainerSize { get; set; } = "45vw"; //if not set, will keep going forever.
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public string TargetImageSize { get; set; } = "10vh";
    [Parameter]
    public EnumMode Mode { get; set; }
}