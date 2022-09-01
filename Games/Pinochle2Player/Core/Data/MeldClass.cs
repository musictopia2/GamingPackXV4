namespace Pinochle2Player.Core.Data;
public class MeldClass
{
    public int Player { get; set; }
    public EnumClassA ClassAValue { get; set; } = EnumClassA.None;
    public EnumClassB ClassBValue { get; set; } = EnumClassB.None;
    public EnumClassC ClassCValue { get; set; } = EnumClassC.None;
    public BasicList<int> CardList { get; set; } = new();
}