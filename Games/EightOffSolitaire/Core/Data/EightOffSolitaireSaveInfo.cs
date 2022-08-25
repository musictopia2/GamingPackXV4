namespace EightOffSolitaire.Core.Data;
[SingletonGame]
[Cloneable(explicitDeclaration: false)]
public class EightOffSolitaireSaveInfo : SolitaireSavedClass, IMappable
{
    public DeckRegularDict<SolitaireCard> ReserveList { get; set; } = new();
}