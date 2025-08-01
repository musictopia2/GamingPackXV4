namespace BowlingDiceGame.Core.Data;
public readonly partial record struct EnumLuckProfile
{
    private enum EnumInfo
    {
        HotestStreak, SomewhatHotStreak, VeryStreaky,
        VeryUnluck, SomewhatUnlucky, VeryBalanced
    }
}