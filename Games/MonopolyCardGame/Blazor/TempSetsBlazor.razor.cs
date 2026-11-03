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
    
    [CascadingParameter]
    public int ImageHeight { get; set; } = 14;
    [Parameter]
    public int HeightPixels { get; set; }
    private int CardHeightPixels => HeightPixels > 0
        ? HeightPixels
        : BasicGameFrameworkLibrary.Blazor.Helpers.LayoutInfo.GetHeight(ImageHeight);

    [Parameter]
    public EnumMode Mode { get; set; }
}