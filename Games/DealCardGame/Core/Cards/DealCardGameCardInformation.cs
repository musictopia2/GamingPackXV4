namespace DealCardGame.Core.Cards;
public class DealCardGameCardInformation : SimpleDeckObject, IDeckObject, IComparable<DealCardGameCardInformation>
{
    public int ClaimedValue { get; set; }
    public int RentAmount { get; set; }
    public EnumColor MainColor { get; set; } = EnumColor.None; //defaults to none.  this represents the group
    public bool AnyColor { get; set; } //sometimes something can represent any color
    public EnumColor FirstColorChoice { get; set; } = EnumColor.None;
    public EnumColor SecondColorChoice { get; set; } = EnumColor.None;
    public EnumCardType CardType { get; set; } = EnumCardType.None; //has to figure out what this is.
    public EnumActionCategory ActionCategory { get; set; } = EnumActionCategory.None;
    public DealCardGameCardInformation()
    {
        DefaultSize = new SizeF(55, 72); //this is neeeded too.
    }
    public void Populate(int chosen)
    {
        //populating the card.
        if (chosen < 0)
        {
            throw new CustomBasicException("Obviously cannot be less than 0");
        }
        if (chosen == 0)
        {
            throw new CustomBasicException("Cannot choose 0");
        }
        Deck = chosen; //for now, just this until i make more progress.
        if (chosen <= 20)
        {
            CardType = EnumCardType.Money;
            if (chosen <= 6)
            {
                ClaimedValue = 1;
                return;
            }
            if (chosen <= 11)
            {
                ClaimedValue = 2;
                return;
            }
            if (chosen <= 14)
            {
                ClaimedValue = 3;
                return;
            }
            if (chosen <= 17)
            {
                ClaimedValue = 4;
                return;
            }
            if (chosen <= 19)
            {
                ClaimedValue = 5;
                return;
            }
            if (chosen == 20)
            {
                ClaimedValue = 10;
                return;
            }
            throw new CustomBasicException("Nothing found for money");
        }
        if (chosen <= 48)
        {
            CardType = EnumCardType.PropertyRegular;
            if (chosen <= 22)
            {
                MainColor = EnumColor.Brown;
                ClaimedValue = 1;
                return;
            }
            if (chosen <= 25)
            {
                MainColor = EnumColor.Cyan;
                ClaimedValue = 1;
                return;
            }
            if (chosen <= 28)
            {
                MainColor = EnumColor.MediumVioletRed;
                ClaimedValue = 2;
                return;
            }
            if (chosen <= 31)
            {
                MainColor = EnumColor.DarkOrange;
                ClaimedValue = 2;
                return;
            }
            if (chosen <= 34)
            {
                MainColor = EnumColor.Red;
                ClaimedValue = 3;
                return;
            }
            if (chosen <= 37)
            {
                MainColor = EnumColor.Yellow;
                ClaimedValue = 3;
                return;
            }
            if (chosen <= 40)
            {
                MainColor = EnumColor.Green;
                ClaimedValue = 4;
                return;
            }
            if (chosen <= 42)
            {
                MainColor = EnumColor.DarkBlue;
                ClaimedValue = 4;
                return;
            }
            if (chosen <= 44)
            {
                MainColor = EnumColor.Lime;
                ClaimedValue = 2;
                return;
            }
            MainColor = EnumColor.Black;
            ClaimedValue = 2;
            return;
        }
        if (chosen <= 59)
        {
            CardType = EnumCardType.PropertyWild;
            if (chosen == 49)
            {
                FirstColorChoice = EnumColor.Cyan;
                SecondColorChoice = EnumColor.Brown;
                ClaimedValue = 1;
                return;
            }
            if (chosen == 50)
            {
                FirstColorChoice = EnumColor.Cyan;
                SecondColorChoice = EnumColor.Black;
                ClaimedValue = 2;
                return;
            }
            if (chosen <= 52)
            {
                FirstColorChoice = EnumColor.MediumVioletRed;
                SecondColorChoice = EnumColor.DarkOrange;
                ClaimedValue = 2;
                return;
            }
            if (chosen <= 54)
            {
                FirstColorChoice = EnumColor.Red;
                SecondColorChoice = EnumColor.Yellow;
                ClaimedValue = 3;
                return;
            }
            if (chosen == 55)
            {
                FirstColorChoice = EnumColor.Green;
                SecondColorChoice = EnumColor.DarkBlue;
                ClaimedValue = 4;
                return;
            }
            if (chosen == 56)
            {
                FirstColorChoice = EnumColor.Black;
                SecondColorChoice = EnumColor.Green;
                ClaimedValue = 4;
                return;
            }
            if (chosen == 57)
            {
                FirstColorChoice = EnumColor.Lime;
                SecondColorChoice = EnumColor.Black;
                ClaimedValue = 2;
                return;
            }
            AnyColor = true;
            ClaimedValue = 0;
            return;
        }
        if (chosen <= 72)
        {
            CardType = EnumCardType.ActionRent;
            ClaimedValue = 1;
            if (chosen <= 61)
            {
                FirstColorChoice = EnumColor.Brown;
                SecondColorChoice = EnumColor.Cyan;
                return;
            }
            if (chosen <= 63)
            {
                FirstColorChoice = EnumColor.MediumVioletRed;
                SecondColorChoice = EnumColor.DarkOrange;
                return;
            }
            if (chosen <= 65)
            {
                FirstColorChoice = EnumColor.Red;
                SecondColorChoice = EnumColor.Yellow;
                return;
            }
            if (chosen <= 67)
            {
                FirstColorChoice = EnumColor.Green;
                SecondColorChoice = EnumColor.DarkBlue;
                return;
            }
            if (chosen <= 69)
            {
                FirstColorChoice = EnumColor.Lime;
                SecondColorChoice = EnumColor.Black;
                return;
            }
            AnyColor = true;
            ClaimedValue = 3;
            return;
        }
        if (chosen <= 106)
        {
            CardType = EnumCardType.ActionStandard;
            if (chosen <= 75)
            {
                ActionCategory = EnumActionCategory.House;
                ClaimedValue = 3;
                return;
            }
            if (chosen <= 77)
            {
                ActionCategory = EnumActionCategory.Hotel;
                ClaimedValue = 4;
                return;
            }
            if (chosen <= 79)
            {
                ActionCategory = EnumActionCategory.DoubleRent;
                ClaimedValue = 1;
                return;
            }
            if (chosen <= 82)
            {
                ActionCategory = EnumActionCategory.Birthday;
                ClaimedValue = 2;
                return;
            }
            if (chosen <= 85)
            {
                ActionCategory = EnumActionCategory.DebtCollector;
                ClaimedValue = 3;
                return;
            }
            if (chosen <= 88)
            {
                ActionCategory = EnumActionCategory.ForcedDeal;
                ClaimedValue = 3;
                return;
            }
            if (chosen <= 91)
            {
                ActionCategory = EnumActionCategory.SlyDeal;
                ClaimedValue = 3;
                return;
            }
            if (chosen <= 94)
            {
                ActionCategory = EnumActionCategory.JustSayNo;
                ClaimedValue = 4;
                return;
            }
            if (chosen <= 96)
            {
                ActionCategory = EnumActionCategory.DealBreaker;
                ClaimedValue = 5;
                return;
            }
            ActionCategory = EnumActionCategory.Gos;
            ClaimedValue = 1;
            return;
        }
        throw new CustomBasicException("Nothing found");
    }

    public void Reset()
    {
        //anything that is needed to reset.
    }

    int IComparable<DealCardGameCardInformation>.CompareTo(DealCardGameCardInformation? other)
    {
        //for now, by deck until i make more progress.
        return Deck.CompareTo(other!.Deck);
    }
}