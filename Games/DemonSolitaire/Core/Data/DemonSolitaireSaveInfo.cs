namespace DemonSolitaire.Core.Data;
[SingletonGame]
[Cloneable(explicitDeclaration: false)]
public class DemonSolitaireSaveInfo : SolitaireSavedClass, IMappable
{
    public BasicList<int> HeelList { get; set; } = new();
}