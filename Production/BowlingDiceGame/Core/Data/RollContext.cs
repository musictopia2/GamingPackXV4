namespace BowlingDiceGame.Core.Data;
public static class RollContext
{
    public static RollDistribution? CurrentDistribution { get; set; }

    // 1 = first roll, 2 = second roll, 3 = third roll
    public static int CurrentRollNumber { get; set; }
    public static int HowManyPins { get; set; } = 10; // default to 10 pins for bowling
}