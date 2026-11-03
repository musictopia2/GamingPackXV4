namespace LifeBoardGame.Blazor;
public partial class LifeHandBlazor
{
    [Parameter]
    public HandObservable<LifeBaseCard>? Hand { get; set; }
    /// <summary>
    /// this is where you usually set a percentage which represents how high or wide the container is.
    /// if hand type is horizontal, then its the width
    /// otherwise, its the height
    /// </summary>
    [Parameter]
    public double HeightPercent { get; set; } = 0;

    private string HeightPixels => HeightPercent.HeightPixels;


    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
}