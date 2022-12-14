namespace Xactika.Core.Cards;
public class XactikaCardInformation : SimpleDeckObject, IDeckObject, ITrickCard<EnumShapes>, IComparable<XactikaCardInformation>
{
    public XactikaCardInformation()
    {
        DefaultSize = new SizeF(80, 100);
    }
    public int Player { get; set; }
    public PointF Location { get; set; }
    public int Value { get; set; }
    public int HowManyBalls { get; set; }
    public int HowManyCubes { get; set; }
    public int HowManyCones { get; set; }
    public int HowManyStars { get; set; }
    public int OrderPlayed { get; set; }
    public EnumShapes ShapeUsed { get; set; }
    public int ReadMainValue => Value;
    public EnumShapes GetSuit => ShapeUsed;
    public int GetPoints => Value;
    public bool IsObjectWild => false;
    public void Populate(int chosen)
    {
        Deck = chosen;
        switch (Deck)
        {
            case 1:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 4;
                    break;
                }

            case 2:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 5;
                    break;
                }

            case 3:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 5;
                    break;
                }

            case 4:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 5;
                    break;
                }

            case 5:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 5;
                    break;
                }

            case 6:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 6;
                    break;
                }

            case 7:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 6;
                    break;
                }

            case 8:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 6;
                    break;
                }

            case 9:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 6;
                    break;
                }

            case 10:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 6;
                    break;
                }

            case 11:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 6;
                    break;
                }

            case 12:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 6;
                    break;
                }

            case 13:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 6;
                    break;
                }

            case 14:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 6;
                    break;
                }

            case 15:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 6;
                    break;
                }

            case 16:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 7;
                    break;
                }

            case 17:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 7;
                    break;
                }

            case 18:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 7;
                    break;
                }

            case 19:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 7;
                    break;
                }

            case 20:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 7;
                    break;
                }

            case 21:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 7;
                    break;
                }

            case 22:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 7;
                    break;
                }

            case 23:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 7;
                    break;
                }

            case 24:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 7;
                    break;
                }

            case 25:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 7;
                    break;
                }

            case 26:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 7;
                    break;
                }

            case 27:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 7;
                    break;
                }

            case 28:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 7;
                    break;
                }

            case 29:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 7;
                    break;
                }

            case 30:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 7;
                    break;
                }

            case 31:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 7;
                    break;
                }

            case 32:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 8;
                    break;
                }

            case 33:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 8;
                    break;
                }

            case 34:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 8;
                    break;
                }

            case 35:
                {
                    HowManyBalls = 1;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 8;
                    break;
                }

            case 36:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 8;
                    break;
                }

            case 37:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 8;
                    break;
                }

            case 38:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 8;
                    break;
                }

            case 39:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 8;
                    break;
                }

            case 40:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 8;
                    break;
                }

            case 41:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 8;
                    break;
                }

            case 42:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 8;
                    break;
                }

            case 43:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 8;
                    break;
                }

            case 44:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 8;
                    break;
                }

            case 45:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 8;
                    break;
                }

            case 46:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 8;
                    break;
                }

            case 47:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 1;
                    Value = 8;
                    break;
                }

            case 48:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 8;
                    break;
                }

            case 49:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 8;
                    break;
                }

            case 50:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 8;
                    break;
                }

            case 51:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 9;
                    break;
                }

            case 52:
                {
                    HowManyBalls = 1;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 9;
                    break;
                }

            case 53:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 9;
                    break;
                }

            case 54:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 9;
                    break;
                }

            case 55:
                {
                    HowManyBalls = 2;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 9;
                    break;
                }

            case 56:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 9;
                    break;
                }

            case 57:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 9;
                    break;
                }

            case 58:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 9;
                    break;
                }

            case 59:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 9;
                    break;
                }

            case 60:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 2;
                    Value = 9;
                    break;
                }

            case 61:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 9;
                    break;
                }

            case 62:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 9;
                    break;
                }

            case 63:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 9;
                    break;
                }

            case 64:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 1;
                    Value = 9;
                    break;
                }

            case 65:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 9;
                    break;
                }

            case 66:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 9;
                    break;
                }

            case 67:
                {
                    HowManyBalls = 1;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 10;
                    break;
                }

            case 68:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 10;
                    break;
                }

            case 69:
                {
                    HowManyBalls = 2;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 10;
                    break;
                }

            case 70:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 10;
                    break;
                }

            case 71:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 1;
                    HowManyStars = 3;
                    Value = 10;
                    break;
                }

            case 72:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 10;
                    break;
                }

            case 73:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 2;
                    Value = 10;
                    break;
                }

            case 74:
                {
                    HowManyBalls = 3;
                    HowManyCones = 1;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 10;
                    break;
                }

            case 75:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 10;
                    break;
                }

            case 76:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 1;
                    Value = 10;
                    break;
                }

            case 77:
                {
                    HowManyBalls = 2;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 11;
                    break;
                }

            case 78:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 2;
                    HowManyStars = 3;
                    Value = 11;
                    break;
                }

            case 79:
                {
                    HowManyBalls = 3;
                    HowManyCones = 2;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 11;
                    break;
                }

            case 80:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 2;
                    Value = 11;
                    break;
                }

            case 81:
                {
                    HowManyBalls = 3;
                    HowManyCones = 3;
                    HowManyCubes = 3;
                    HowManyStars = 3;
                    Value = 12;
                    break;
                }
        }
        if (Value == 0)
        {
            throw new Exception("Can't find the deck " + Deck);
        }
    }
    public void Reset()
    {
        ShapeUsed = EnumShapes.None;
    }
    public object CloneCard()
    {
        return MemberwiseClone();
    }
    int IComparable<XactikaCardInformation>.CompareTo(XactikaCardInformation? other)
    {
        return Value.CompareTo(other!.Value);
    }
}