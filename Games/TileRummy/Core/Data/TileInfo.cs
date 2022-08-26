namespace TileRummy.Core.Data;
public class TileInfo : SimpleDeckObject, IDeckObject, IRummmyObject<EnumColorType, EnumColorType>, IComparable<TileInfo>, ILocationDeck
{
    public PointF Location { get; set; }
    public bool IsJoker { get; set; }
    public TileInfo()
    {
        DefaultSize = new SizeF(60, 40);
    }
    public int Points { get; set; }
    public EnumDrawType WhatDraw { get; set; } = EnumDrawType.IsNone;
    public EnumColorType Color { get; set; }
    public int Number { get; set; }
    int IRummmyObject<EnumColorType, EnumColorType>.GetSecondNumber => Number;
    int ISimpleValueObject<int>.ReadMainValue => Number;
    bool IWildObject.IsObjectWild => IsJoker;
    bool IIgnoreObject.IsObjectIgnored => false;
    EnumColorType ISuitObject<EnumColorType>.GetSuit => Color;
    EnumColorType IColorObject<EnumColorType>.GetColor => Color;
    public void Populate(int chosen)
    {
        int x;
        int y;
        int z;
        int q = 0;
        for (x = 1; x <= 4; x++)
        {
            for (y = 1; y <= 13; y++)
            {
                for (z = 1; z <= 2; z++)
                {
                    q += 1;
                    if (q == chosen)
                    {
                        Deck = chosen;
                        if (x == 1)
                        {
                            Color = EnumColorType.Black;
                        }
                        else if (x == 2)
                        {
                            Color = EnumColorType.Blue;
                        }
                        else if (x == 3)
                        {
                            Color = EnumColorType.Orange;
                        }
                        else if (x == 4)
                        {
                            Color = EnumColorType.Red;
                        }
                        Number = y;
                        Points = y;
                        IsJoker = false;
                        return;
                    }
                }
            }
        }
        for (x = 105; x <= 106; x++)
        {
            if (chosen == x)
            {
                Deck = chosen;
                IsJoker = true;
                Number = 20;
                Points = 25;
                if (x == 105)
                {
                    Color = EnumColorType.Black;
                }
                else
                {
                    Color = EnumColorType.Red;
                }
                return;
            }
        }
        throw new CustomBasicException("Cannot find the deck " + Deck);
    }
    public void Reset()
    {
        WhatDraw = EnumDrawType.IsNone;
    }
    int IComparable<TileInfo>.CompareTo(TileInfo? other)
    {
        if (Number != other!.Number)
        {
            return Number.CompareTo(other.Number);
        }
        return Color.CompareTo(other.Color);
    }
}
