namespace BasicGameFrameworkLibrary.Core.Dice;

public class SimpleDice : IStandardDice, IGenerateDice<int>, ISimpleValueObject<int>
{
    public int HeightWidth { get; } = 60; //for now does not matter.
    public string DotColor { get; set; } = cs.Black; //you have to make it public.  otherwise, you can't save the color which is needed for games like kismet.
    public string FillColor { get; set; } = cs.White;
    public int Value { get; set; }
    public int Index { get; set; }
    public EnumDiceStyle Style { get; } = EnumDiceStyle.Regular;
    public bool Hold { get; set; }
    public bool IsSelected { get; set; }
    public bool Visible { get; set; } = true;
    public bool IsEnabled { get; set; } = true; //try this too.
    BasicList<int> IGenerateDice<int>.GetPossibleList => Enumerable.Range(1, 6).ToBasicList();
    int ISimpleValueObject<int>.ReadMainValue => Value;
    public virtual void Populate(int Chosen)
    {
        Value = Chosen;
    }
}