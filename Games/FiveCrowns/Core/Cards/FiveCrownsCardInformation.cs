namespace FiveCrowns.Core.Cards;
public class FiveCrownsCardInformation : SimpleDeckObject, IDeckObject, IRummmyObject<EnumSuitList, EnumColorList>, IComparable<FiveCrownsCardInformation>
{
    public FiveCrownsCardInformation()
    {
        DefaultSize = new SizeF(165, 216);
    }
    int IRummmyObject<EnumSuitList, EnumColorList>.GetSecondNumber => CardValue.Value;
    int ISimpleValueObject<int>.ReadMainValue => CardValue.Value;

    private FiveCrownsMainGameClass? _mainGame;
    public bool IsObjectWild
    {
        get
        {
            _mainGame ??= aa1.Resolver!.Resolve<FiveCrownsMainGameClass>();
            return CardType == EnumCardTypeList.Joker || _mainGame.SaveRoot!.UpTo == CardValue.Value;
        }
    }
    bool IIgnoreObject.IsObjectIgnored => false;
    EnumSuitList ISuitObject<EnumSuitList>.GetSuit => Suit;
    EnumColorList IColorObject<EnumColorList>.GetColor => ColorSuit;
    public EnumSuitList Suit { get; set; } = EnumSuitList.None;
    public EnumSuitList OriginalSuit { get; set; } = EnumSuitList.None;
    public EnumCardValueList CardValue { get; set; }
    public EnumCardTypeList CardType { get; set; }
    public EnumColorList ColorSuit { get; set; }
    public int Points { get; set; }
    public void Populate(int chosen)
    {
        int newItem;
        if (chosen > 58)
        {
            newItem = chosen - 58;
        }
        else
        {
            newItem = chosen;
        }
        if (newItem >= 56)
        {
            Deck = chosen;
            CardType = EnumCardTypeList.Joker;
            CardValue = EnumCardValueList.Joker;
            return;
        }
        int x;
        int y;
        int z = 0;
        for (x = 1; x <= 5; x++)
        {
            for (y = 3; y <= 13; y++)
            {
                z += 1;
                if (z == newItem)
                {

                    Suit = EnumSuitList.FromValue(x);
                    Deck = chosen;
                    OriginalSuit = Suit; //this too.
                    CardValue = EnumCardValueList.FromValue(y);
                    if (Suit == EnumSuitList.Clubs)
                    {
                        ColorSuit = EnumColorList.Green;
                    }
                    else if (Suit == EnumSuitList.Diamonds)
                    {
                        ColorSuit = EnumColorList.Blue;
                    }
                    else if (Suit == EnumSuitList.Spades)
                    {
                        ColorSuit = EnumColorList.Black;
                    }
                    else if (Suit == EnumSuitList.Hearts)
                    {
                        ColorSuit = EnumColorList.Red;
                    }
                    else if (Suit == EnumSuitList.Stars)
                    {
                        ColorSuit = EnumColorList.Yellow;
                    }
                    else
                    {
                        throw new CustomBasicException("Not supported");
                    }
                    return;
                }
            }
        }
        throw new CustomBasicException($"Nothing found for chosen {chosen}");
    }
    public void Reset()
    {
        Suit = OriginalSuit;
    }
    int IComparable<FiveCrownsCardInformation>.CompareTo(FiveCrownsCardInformation? other)
    {
        if (IsObjectWild == true && other!.IsObjectWild == false)
        {
            return 1;
        }
        else if (IsObjectWild == false && other!.IsObjectWild == true)
        {
            return -1;
        }
        if (ColorSuit != other!.ColorSuit)
        {
            return ColorSuit.CompareTo(other.ColorSuit);
        }
        return CardValue.CompareTo(other.CardValue);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}