namespace LittleSpiderSolitaire.Blazor;
public partial class CustomWasteUI
{
    [Parameter]
    public BasicMultiplePilesCP<SolitaireCard>? MainPiles { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<SolitaireCard>? WastePiles { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
}