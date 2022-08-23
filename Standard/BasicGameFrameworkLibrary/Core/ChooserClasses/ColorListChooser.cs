namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public class ColorListChooser<E> : IEnumListClass<E> where E : struct, IFastEnumColorList<E>
{
    BasicList<E> IEnumListClass<E>.GetEnumList()
    {
        E newE = new(); //okay because its struct.
        var output = newE.ColorList; //this will automatically not have none or any other custom stuff or even zother for some games.
        return output;
    }
}