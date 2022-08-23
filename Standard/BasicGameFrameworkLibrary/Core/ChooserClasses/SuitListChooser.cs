namespace BasicGameFrameworkLibrary.Core.ChooserClasses;

public class SuitListChooser : IEnumListClass<EnumSuitList>
{
    BasicList<EnumSuitList> IEnumListClass<EnumSuitList>.GetEnumList()
    {
        return new BasicList<EnumSuitList>()
    { EnumSuitList.Clubs,
    EnumSuitList.Diamonds, EnumSuitList.Hearts, EnumSuitList.Spades};
    }
}