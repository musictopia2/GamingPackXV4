namespace FourSuitRummy.Core.Data;
public class SetInfo : SetInfo<EnumSuitList, EnumRegularColorList, RegularRummyCard, SavedSet>
{
    public SetInfo(CommandContainer command) : base(command) { }
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