namespace Mastermind.Core.Logic;
public class CustomColorClass(GlobalClass global) : IEnumListClass<EnumColorPossibilities>
{
    BasicList<EnumColorPossibilities> IEnumListClass<EnumColorPossibilities>.GetEnumList()
    {
        return global.ColorList;
    }
}