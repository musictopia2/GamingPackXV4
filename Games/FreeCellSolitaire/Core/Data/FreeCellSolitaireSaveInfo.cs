namespace FreeCellSolitaire.Core.Data;
[SingletonGame]
[Cloneable(explicitDeclaration: false)]
public class FreeCellSolitaireSaveInfo : SolitaireSavedClass, IMappable
{
    public BasicList<BasicPileInfo<SolitaireCard>> FreeCards = new();
}