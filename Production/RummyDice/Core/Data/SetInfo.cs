namespace RummyDice.Core.Data;
public class SetInfo
{
    public EnumWhatSets SetType;
    public bool HasLaid;
    public bool DidSucceed;
    public int HowMany;
    public BasicList<RummyDiceInfo>? SetCards;
}