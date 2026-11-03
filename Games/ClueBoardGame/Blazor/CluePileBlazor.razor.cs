namespace ClueBoardGame.Blazor;
public partial class CluePileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 22;
    [Parameter]
    public SingleObservablePile<CardInfo>? SinglePile { get; set; }
}