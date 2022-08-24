namespace BasicGameFrameworkLibrary.Blazor.BasicControls.MultipleFrameContainers;
public partial class DeckOfCardsMultiplePilesBlazor<R>
    where R : class, IRegularCard, new()
{
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<R>? Piles { get; set; }
    [Parameter]
    public string AnimationTag { get; set; } = "";
    [Parameter]
    public bool Inline { get; set; } = true;
}