namespace HeapSolitaire.Blazor;
public partial class SinglePileUI
{

    [Parameter]
    public BasicPileInfo<HeapSolitaireCardInfo>? Pile { get; set; }
}