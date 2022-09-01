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
    public string TargetContainerSize { get; set; } = ""; //if not set, will keep going forever.
}