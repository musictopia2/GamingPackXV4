﻿namespace BasicGameFrameworkLibrary.Core.Dice;
public class SimpleDice : IStandardDice, IGenerateDice<int>, ISimpleValueObject<int>
{
    public int HeightWidth { get; } = 60;
    public string DotColor { get; set; } = cs1.Black; //you have to make it public.  otherwise, you can't save the color which is needed for games like kismet.
    public string FillColor { get; set; } = cs1.White;
    public int Value { get; set; }
    public int Index { get; set; }
    public EnumDiceStyle Style { get; } = EnumDiceStyle.Regular;
    public bool Hold { get; set; }
    public bool IsSelected { get; set; }
    public bool Visible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
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