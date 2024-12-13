namespace BasicGameFrameworkLibrary.Core.Dice;
public class TenSidedDice : BaseSpecialStyleDice
{
    public override string DotColor { get; set; } = cs1.Black; //has to be public.  or autoresume does not work.  learned from kismet.
    public override string FillColor { get; set; } = cs1.Blue;
    public override int GetRandomDiceValue(bool isLastItem)
    {
        BasicList<int> list = Enumerable.Range(1, 10).ToBasicList();
        return list.GetRandomItem();
    }
}