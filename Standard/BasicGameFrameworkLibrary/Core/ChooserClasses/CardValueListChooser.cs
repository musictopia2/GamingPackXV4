namespace BasicGameFrameworkLibrary.Core.ChooserClasses;

public class CardValueListChooser : IEnumListClass<EnumRegularCardValueList>
{
    BasicList<EnumRegularCardValueList> IEnumListClass<EnumRegularCardValueList>.GetEnumList()
    {
        return new BasicList<EnumRegularCardValueList>()
    {
        EnumRegularCardValueList.LowAce, EnumRegularCardValueList.Two, EnumRegularCardValueList.Three, EnumRegularCardValueList.Four,
        EnumRegularCardValueList.Five, EnumRegularCardValueList.Six, EnumRegularCardValueList.Seven, EnumRegularCardValueList.Eight,
        EnumRegularCardValueList.Nine, EnumRegularCardValueList.Ten, EnumRegularCardValueList.Jack, EnumRegularCardValueList.Queen,
        EnumRegularCardValueList.King
    };
    }
}