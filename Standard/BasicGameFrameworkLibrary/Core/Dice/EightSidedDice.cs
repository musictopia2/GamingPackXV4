namespace BasicGameFrameworkLibrary.Core.Dice;
public class EightSidedDice : BaseSpecialStyleDice
{
    public override string DotColor { get; set; } = cs1.Black;
    public override string FillColor { get; set; } = cs1.Green;
    public override int GetRandomDiceValue(bool isLastItem)
    {
        BasicList<int> list = Enumerable.Range(1, 8).ToBasicList();
        return list.GetRandomItem();
    }
}