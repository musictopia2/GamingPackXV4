namespace TriangleSolitaire.Core.Data;
[SingletonGame]
public class TriangleSolitaireSaveInfo : IMappable
{
    public BasicList<int> DeckList { get; set; } = new();
    public SavedDiscardPile<SolitaireCard>? PileData { get; set; }
    public SavedTriangle? TriangleData { get; set; }
    public EnumIncreaseList Incs { get; set; }
}