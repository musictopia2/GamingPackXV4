namespace LifeBoardGame.Blazor;
public partial class LifePileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 18;
    [Parameter]
    public SingleObservablePile<LifeBaseCard>? SinglePile { get; set; }
    
}