namespace EagleWingsSolitaire.Core.Data;
[SingletonGame]
[Cloneable(explicitDeclaration: false)]
public class EagleWingsSolitaireSaveInfo : SolitaireSavedClass, IMappable
{
    public BasicList<int> HeelList { get; set; } = new();
}