namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.DrawPiles;
public partial class DeckOfCardsDrawPileBlazor<R>
    where R : class, IRegularCard, new()
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public DeckObservablePile<R>? DeckPile { get; set; }
    [Parameter]
    public string DeckAnimationTag { get; set; } = "maindeck";
    private string RealHeight => $"{TargetHeight}vh";
    private bool AlwaysUnknown => DeckPile!.DeckStyle == EnumDeckPileStyle.Unknown;
}