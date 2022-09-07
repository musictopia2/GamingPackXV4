namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public class RegularSimpleCard : SimpleDeckObject, IRegularCard
{
    public RegularSimpleCard()
    {
        DefaultSize = new SizeF(165, 216);
    }
    public EnumRegularCardValueList Value { get; set; }
    public EnumSuitList Suit { get; set; }
    public EnumSuitList DisplaySuit { get; set; } = EnumSuitList.None;
    public EnumRegularCardValueList DisplayNumber { get; set; } = EnumRegularCardValueList.None;
    public EnumRegularCardTypeList CardType { get; set; }
    public EnumRegularColorList Color { get; set; }
    public int Points { get; set; }
    public EnumSuitList GetSuit => Suit;
    public EnumRegularColorList GetColor => Color;
    public int Section { get; private set; }
    protected static IRegularDeckInfo? ThisInfo { get; set; } //use dependency injection to populate this.
    protected static IRegularAceCalculator? ThisAce { get; set; }
    public static void ClearSavedList()
    {
        ThisInfo = null;
        ThisAce = null;
        _list.Clear();
    }
    public IGamePackageResolver? MainContainer { get; set; }
    int ISimpleValueObject<int>.ReadMainValue => Value.Value;
    private static IRegularDeckWild? _thisWild;
    private void SetObjects()
    {
        if (MainContainer != null && ThisInfo != null)
        {
            return;
        }
        PopulateContainer(this);
        if (ThisInfo != null)
        {
            return;
        }
        ThisInfo = MainContainer!.Resolve<IRegularDeckInfo>();
        ThisAce = MainContainer.Resolve<IRegularAceCalculator>();
        if (MainContainer.RegistrationExist<IRegularDeckWild>() == true)
        {
            _thisWild = MainContainer.Resolve<IRegularDeckWild>();
        }
    }
    public virtual bool IsObjectWild
    {
        get
        {
            SetObjects();
            if (ThisInfo!.UseJokers == false)
            {
                return false;
            }
            if (_thisWild != null)
            {
                return _thisWild.IsWild(this);
            }
            return Value == EnumRegularCardValueList.Joker;
        }
    }
    private static bool CanUse(EnumSuitList thisSuit, int number)
    {
        if (ThisInfo!.ExcludeList.Count == 0)
        {
            return true;
        }
        return !ThisInfo.ExcludeList.Any(exclude =>
        {

            if (exclude.Number == 1 || exclude.Number == 14)
            {
                if (number == 1 || number == 14)
                {
                    return exclude.Suit == thisSuit;
                }
                else
                {
                    return false; //i think
                }
            }
            else
            {
                return exclude.Number == number && exclude.Suit == thisSuit;
            }
        });
    }
    protected virtual void FinishPopulatingCard() { }
    protected virtual void PopulateAceValue() //for games like chinazo, i can do something else.  since i am already inheriting to add extras.
    {
        ThisAce!.PopulateAceValues(this);
    }
    private static readonly DeckRegularDict<RegularSimpleCard> _list = new();
    public void Populate(int chosen)
    {
        SetObjects();
        if (_list.Count == 0)
        {
            GenerateList();
        }
        var item = _list.GetSpecificItem(chosen);
        Deck = chosen;
        Suit = item.Suit;
        Section = item.Section;
        Value = item.Value;
        Color = item.Color;
        CardType = item.CardType;
        FinishPopulatingCard();
    }
    private void GenerateList()
    {
        _list.Clear();
        int x, y, z, q = 0;
        for (x = 1; x <= ThisInfo!.HowManyDecks; x++)
        {
            for (y = 0; y < ThisInfo.SuitList.Count; y++)
            {
                for (z = ThisInfo.LowestNumber; z <= ThisInfo.HighestNumber; z++)
                {
                    if (CanUse(ThisInfo.SuitList[y], z))
                    {
                        q++;
                        RegularSimpleCard card = new();
                        card.Deck = q;
                        card.Suit = ThisInfo.SuitList[y];
                        if (z == 1 || z == 14)
                        {
                            PopulateAceValue();
                        }
                        else
                        {
                            Value = EnumRegularCardValueList.FromValue(z);
                        }
                        card.Value = Value;
                        card.Section = x;
                        card.CardType = EnumRegularCardTypeList.Regular;
                        if (card.Suit == EnumSuitList.Clubs || card.Suit == EnumSuitList.Spades)
                        {
                            card.Color = EnumRegularColorList.Black;
                        }
                        else
                        {
                            card.Color = EnumRegularColorList.Red;
                        }
                        _list.Add(card);
                    }
                }
            }
        }
        if (ThisInfo.UseJokers == true)
        {
            for (int r = 1; r <= ThisInfo.HowManyDecks; r++)
            {
                for (int p = 1; p <= 2; p++)
                {
                    q++;
                    RegularSimpleCard card = new();
                    card.Value = EnumRegularCardValueList.Joker;
                    card.Deck = q;
                    card.CardType = EnumRegularCardTypeList.Joker;
                    if (p == 1)
                    {
                        card.Color = EnumRegularColorList.Black;
                    }
                    else
                    {
                        card.Color = EnumRegularColorList.Red;
                    }
                    card.Section = r;
                    _list.Add(card);
                }
            }
            EnumRegularColorList last = EnumRegularColorList.Red;
            int s = ThisInfo.HowManyDecks;
            for (int i = 0; i < ThisInfo.GetExtraJokers; i++)
            {
                q++;
                if (last == EnumRegularColorList.Red)
                {
                    last = EnumRegularColorList.Black;
                }
                else
                {
                    last = EnumRegularColorList.Red;
                }
                s++;
                if (s > ThisInfo.HowManyDecks)
                {
                    s = 1;
                }
                RegularSimpleCard card = new();
                card.Value = EnumRegularCardValueList.Joker;
                card.Deck = q;
                card.CardType = EnumRegularCardTypeList.Joker;
                card.Section = s;
                card.Color = last;
                _list.Add(card);
            }
        }
    }
    public void Reset()
    {
        DisplayNumber = EnumRegularCardValueList.None;
        DisplaySuit = EnumSuitList.None;
    }
    public override string ToString()
    {
        if (Deck == 0)
        {
            throw new CustomBasicException("Deck cannot be 0 when populating the tostring");
        }
        if (Value == EnumRegularCardValueList.Joker)
        {
            return $"{Color.Name} Joker";
        }
        if (Value == EnumRegularCardValueList.Continue || Value == EnumRegularCardValueList.Stop)
        {
            return Value.Name;
        }
        EnumSuitList tempSuit;
        if (DisplaySuit == EnumSuitList.None)
        {
            tempSuit = Suit;
        }
        else
        {
            tempSuit = DisplaySuit;
        }
        EnumRegularCardValueList tempValue;
        if (DisplayNumber == EnumRegularCardValueList.None)
        {
            tempValue = Value;
        }
        else
        {
            tempValue = DisplayNumber;
        }
        string stringValue;
        if (tempValue == EnumRegularCardValueList.HighAce || tempValue == EnumRegularCardValueList.LowAce)
        {
            stringValue = "A";
        }
        else
        {
            stringValue = tempValue.ToString();
        }
        string stringSuit = GetStringSuit(tempSuit);
        return $"{stringValue} {stringSuit} Deck {Deck}";
    }
    private static string GetStringSuit(EnumSuitList tempSuit)
    {
        if (tempSuit == EnumSuitList.None)
        {
            throw new CustomBasicException("Suit Can't be None");
        }
        if (tempSuit == EnumSuitList.Clubs)
        {
            return "C♣︎";
        }
        if (tempSuit == EnumSuitList.Diamonds)
        {
            return "D♦︎";
        }
        if (tempSuit == EnumSuitList.Spades)
        {
            return "S♠";
        }
        if (tempSuit == EnumSuitList.Hearts)
        {
            return "H♥";
        }
        throw new CustomBasicException("Not Supported");
    }
    public override BasicDeckRecordModel GetRecord => new(Deck, IsSelected, Drew, IsUnknown, IsEnabled, Visible, $"{DisplayNumber} {DisplaySuit}");
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public override string GetKey()
    {
        if (Value != EnumRegularCardValueList.Joker)
        {
            return Deck.ToString();
        }
        return Guid.NewGuid().ToString();
    }
}