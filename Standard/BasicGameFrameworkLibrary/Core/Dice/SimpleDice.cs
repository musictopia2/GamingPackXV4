namespace BasicGameFrameworkLibrary.Core.Dice;
[UseStateChanged]
public partial class SimpleDice : IStandardDice, IGenerateDice<int>, ISimpleValueObject<int>
{
    public int HeightWidth { get; } = 60;
    [StateProperty]
    public partial string DotColor { get; set; } = cs1.Black; //you have to make it public.  otherwise, you can't save the color which is needed for games like kismet.
    [StateProperty]
    public partial string FillColor { get; set; } = cs1.White;
    [StateProperty]
    public partial int Value { get; set; }
    [StateProperty]
    public partial int Index { get; set; }

    public EnumDiceStyle Style { get; } = EnumDiceStyle.Regular;
    [StateProperty]
    public partial bool Hold { get; set; }
    [StateProperty]
    public partial bool IsSelected { get; set; }
    [StateProperty]
    public partial bool Visible { get; set; } = true;
    [StateProperty]
    public partial bool IsEnabled { get; set; } = true;
    int ISimpleValueObject<int>.ReadMainValue => Value;
    public virtual void Populate(int chosen)
    {
        Value = chosen;
    }
    int IGenerateDice<int>.GetRandomDiceValue(bool isLastItem)
    {
        var list = Enumerable.Range(1, 6).ToBasicList();
        return list.GetRandomItem();
    }
    void IGenerateDice<int>.StartRoll()
    {
        //not needed
    }
}