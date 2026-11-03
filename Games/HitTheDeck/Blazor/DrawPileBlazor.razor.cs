namespace HitTheDeck.Blazor;
public partial class DrawPileBlazor
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 22;
    [Parameter]
    public DeckObservablePile<HitTheDeckCardInformation>? DeckPile { get; set; }
    [Parameter]
    public string DeckAnimationTag { get; set; } = "maindeck";
}