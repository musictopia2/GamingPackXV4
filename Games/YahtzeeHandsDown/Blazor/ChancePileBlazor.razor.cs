namespace YahtzeeHandsDown.Blazor;
public partial class ChancePileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public SingleObservablePile<ChanceCardInfo>? SinglePile { get; set; }
    private string RealHeight => $"{TargetHeight}vh";
}