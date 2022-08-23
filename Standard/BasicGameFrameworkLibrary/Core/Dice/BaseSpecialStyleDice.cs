namespace BasicGameFrameworkLibrary.Core.Dice;
public abstract class BaseSpecialStyleDice : IStandardDice, IGenerateDice<int>, ISelectableObject
{
    public int HeightWidth { get; } = 60;
    public EnumDiceStyle Style { get; } = EnumDiceStyle.DrawWhiteNumber;
    public int Value { get; set; }
    public int Index { get; set; }
    public bool Hold { get; set; }
    public bool IsSelected { get; set; }
    public bool Visible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    [JsonIgnore]
    public abstract BasicList<int> GetPossibleList { get; }
    public abstract string DotColor { get; set; } //has to be public all the way.  otherwise, autoresume does not work.
    public abstract string FillColor { get; set; }
    public virtual void Populate(int chosen)
    {
        Value = chosen;
    }
}