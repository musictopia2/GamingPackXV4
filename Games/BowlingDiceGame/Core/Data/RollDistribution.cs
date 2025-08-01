namespace BowlingDiceGame.Core.Data;
public class RollDistribution()
{
    public int First { get; set; }
    public int Second { get; set; }
    public int Third { get; set; }
    public bool HasThird { get; set; }
    public static RollDistribution Create(int first, int second, int third, bool hasThird)
    {
        var instance = new RollDistribution();
        instance.First = first;
        instance.Second = second;
        instance.Third = third;
        instance.HasThird = hasThird;
        return instance;
    }
}