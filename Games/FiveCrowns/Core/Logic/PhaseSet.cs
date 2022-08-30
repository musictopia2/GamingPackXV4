namespace FiveCrowns.Core.Logic;
public class PhaseSet : SetInfo<EnumSuitList, EnumColorList, FiveCrownsCardInformation, SavedSet>
{
    public PhaseSet(CommandContainer command) : base(command) { }
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