namespace ClueBoardGame.Blazor;
public partial class CluePileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public SingleObservablePile<CardInfo>? SinglePile { get; set; }
    private string RealHeight => $"{TargetHeight}vh";
}