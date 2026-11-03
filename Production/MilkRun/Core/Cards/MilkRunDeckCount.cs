namespace MilkRun.Core.Cards;
public class MilkRunDeckCount : IDeckCount
{
    public int GetDeckCount()
    {
        return MilkRunCardInformation.HowManyKinds * 2;
    }
}