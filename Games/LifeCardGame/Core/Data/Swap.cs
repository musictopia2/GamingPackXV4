namespace LifeCardGame.Core.Data;
public class Swap
{
    public int Player { get; set; }
    public BasicList<int> YourCards { get; set; } = new();
    public BasicList<int> OtherCards { get; set; } = new();
}