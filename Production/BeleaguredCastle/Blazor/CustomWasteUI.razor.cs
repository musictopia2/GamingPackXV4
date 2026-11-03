namespace BeleaguredCastle.Blazor;
public partial class CustomWasteUI
{
    [Parameter]
    public SolitairePilesCP? WastePiles { get; set; }
    [Parameter]
    public BasicMultiplePilesCP<SolitaireCard>? MainPiles { get; set; }
}