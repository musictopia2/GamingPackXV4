namespace CaliforniaJack.Blazor;
public partial class SingleMiscPileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public SingleObservablePile<CaliforniaJackCardInformation>? SinglePile { get; set; }
    [Parameter]
    public string PileAnimationTag { get; set; } = "maindiscard";
    private string RealHeight => $"{TargetHeight}vh";
    private static string GetKey => Guid.NewGuid().ToString();
}