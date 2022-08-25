namespace Mastermind.Core.Logic;
public class CustomColorClass : IEnumListClass<EnumColorPossibilities>
{
    private readonly GlobalClass _global;
    public CustomColorClass(GlobalClass global)
    {
        _global = global;
    }
    BasicList<EnumColorPossibilities> IEnumListClass<EnumColorPossibilities>.GetEnumList()
    {
        return _global.ColorList;
    }
}