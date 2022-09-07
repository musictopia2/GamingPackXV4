namespace BasicGameFrameworkLibrary.Core.Dice;
public class EightSidedDice : BaseSpecialStyleDice
{
    public override string DotColor { get; set; } = cs.Black;
    public override string FillColor { get; set; } = cs.Green;
    public override BasicList<int> GetPossibleList => Enumerable.Range(1, 8).ToBasicList();
}