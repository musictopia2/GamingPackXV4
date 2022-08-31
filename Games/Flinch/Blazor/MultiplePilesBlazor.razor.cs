namespace Flinch.Blazor;
public partial class MultiplePilesBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<FlinchCardInformation>? Piles { get; set; }
    [Parameter]
    public string AnimationTag { get; set; } = "";
    [Parameter]
    public bool Inline { get; set; } = true;
    private string RealHeight => TargetHeight.HeightString();
}