namespace RaglanSolitaire.Core.Data;
[SingletonGame]
[Cloneable(explicitDeclaration: false)]
public class RaglanSolitaireSaveInfo : SolitaireSavedClass, IMappable
{
    public DeckRegularDict<SolitaireCard> StockCards { get; set; } = new ();
}