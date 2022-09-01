namespace HorseshoeCardGame.Blazor;
public partial class DrawPileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public DeckObservablePile<HorseshoeCardGameCardInformation>? DeckPile { get; set; }
    [Parameter]
    public string DeckAnimationTag { get; set; } = "maindeck";
    private string RealHeight => $"{TargetHeight}vh";
}