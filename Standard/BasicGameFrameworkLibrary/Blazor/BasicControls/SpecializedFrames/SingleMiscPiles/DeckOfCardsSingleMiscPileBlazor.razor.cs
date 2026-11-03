namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.SingleMiscPiles;
public partial class DeckOfCardsSingleMiscPileBlazor<R>
    where R : class, IRegularCard, new()
{
    [Parameter]
    public bool IsTesting { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 21;
    [Parameter]
    public SingleObservablePile<R>? SinglePile { get; set; }
    [Parameter]
    public bool UseKey { get; set; } = true;
    [Parameter]
    public string PileAnimationTag { get; set; } = "maindiscard";
    public int HeightPixels => ll1.GetHeight(TargetHeight);

}