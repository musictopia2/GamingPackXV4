namespace LifeBoardGame.Blazor;
public partial class LifePileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public SingleObservablePile<LifeBaseCard>? SinglePile { get; set; }
    private string RealHeight => $"{TargetHeight}vh";
}