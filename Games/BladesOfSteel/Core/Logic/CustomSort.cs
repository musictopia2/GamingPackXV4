﻿namespace BladesOfSteel.Core.Logic;
[SingletonGame]
public class CustomSort : ISortObjects<RegularSimpleCard>
{
    int IComparer<RegularSimpleCard>.Compare(RegularSimpleCard? x, RegularSimpleCard? y)
    {
        if (x!.Color != y!.Color)
        {
            return x.Color.CompareTo(y.Color);
        }
        if (x.Suit != y.Suit)
        {
            return x.Suit.CompareTo(y.Suit);
        }
        return x.Value.CompareTo(y.Value);
    }
}