namespace FiveCrowns.Core.Logic;
public class PhaseSet(CommandContainer command) : SetInfo<EnumSuitList, EnumColorList, FiveCrownsCardInformation, SavedSet>(command)
{
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