namespace BasicGameFrameworkLibrary.Core.Dice;

public class TenSidedDice : BaseSpecialStyleDice
{
    public override string DotColor { get; set; } = cs.Black; //has to be public.  or autoresume does not work.  learned from kismet.
    public override string FillColor { get; set; } = cs.Blue;
    public override BasicList<int> GetPossibleList => Enumerable.Range(1, 10).ToBasicList();
}