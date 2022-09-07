namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public class ColorListChooser<E> : IEnumListClass<E> where E : struct, IFastEnumColorList<E>
{
    BasicList<E> IEnumListClass<E>.GetEnumList()
    {
        E newE = new();
        var output = newE.ColorList;
        return output;
    }
}