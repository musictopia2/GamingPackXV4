namespace DummyRummy.Core.Logic;
public class DummySet : SetInfo<EnumSuitList, EnumRegularColorList, RegularRummyCard, SavedSet>
{
    public DummySet(CommandContainer command) : base(command) { }
    public override void LoadSet(SavedSet payLoad)
    {
        HandList.ReplaceRange(payLoad.CardList);
    }
    public override SavedSet SavedSet()
    {
        SavedSet output = new();
        output.CardList = HandList.ToRegularDeckDict();
        return output;
    }
}