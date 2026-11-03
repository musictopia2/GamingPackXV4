namespace PersianSolitaire.Core.Data;
[SingletonGame]
[Cloneable(explicitDeclaration: false)]
public class PersianSolitaireSaveInfo : SolitaireSavedClass, IMappable
{
    public int DealNumber { get; set; }
}