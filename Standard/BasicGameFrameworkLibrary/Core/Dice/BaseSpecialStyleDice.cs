namespace BasicGameFrameworkLibrary.Core.Dice;

[UseStateChanged]
public abstract partial class BaseSpecialStyleDice : IStandardDice, IGenerateDice<int>, ISelectableObject
{
    public int HeightWidth { get; } = 60;
    public EnumDiceStyle Style { get; } = EnumDiceStyle.DrawWhiteNumber;
    [StateProperty]
    public partial int Value { get; set; }
    public int Index { get; set; }
    [StateProperty]
    public partial bool Hold { get; set; }
    [StateProperty]
    public partial bool IsSelected { get; set; }
    [StateProperty]
    public partial bool Visible { get; set; } = true;
    [StateProperty]
    public partial bool IsEnabled { get; set; } = true;
    public abstract string DotColor { get; set; } //has to be public all the way.  otherwise, autoresume does not work.
    public abstract string FillColor { get; set; }
    public abstract int GetRandomDiceValue(bool isLastItem);
    public virtual void Populate(int chosen)
    {
        Value = chosen;
    }

    void IGenerateDice<int>.StartRoll()
    {
        //nothing needed
    }
}