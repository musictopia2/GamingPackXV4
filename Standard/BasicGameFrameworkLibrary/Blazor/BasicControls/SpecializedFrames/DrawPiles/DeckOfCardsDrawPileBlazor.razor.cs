namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.DrawPiles;
public partial class DeckOfCardsDrawPileBlazor<R>
    where R : class, IRegularCard, new()
{
    [CascadingParameter]
    public int TargetHeight { get; set; } = 21;
    [Parameter]
    public DeckObservablePile<R>? DeckPile { get; set; }
    [Parameter]
    public string DeckAnimationTag { get; set; } = "maindeck";
    [Parameter]
    public EnumSize? SizeCategory { get; set; }
    private bool AlwaysUnknown => DeckPile!.DeckStyle == EnumDeckPileStyle.Unknown;
    private int RealHeight => ll1.GetHeight(TargetHeight);
}