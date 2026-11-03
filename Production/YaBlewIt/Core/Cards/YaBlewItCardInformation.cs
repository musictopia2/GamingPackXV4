namespace YaBlewIt.Core.Cards;
public class YaBlewItCardInformation : SimpleDeckObject, IDeckObject, IComparable<YaBlewItCardInformation>
{
    public int FirstNumber { get; set; }
    public int SecondNumber { get; set; }
    public EnumCardCategory CardCategory { get; set; }
    public EnumColors CardColor { get; set; }
    public YaBlewItCardInformation()
    {
        DefaultSize = new SizeF(55, 72); //this is neeeded too.
    }
    public void Populate(int chosen)
    {
        if (chosen <= 0 || chosen > 64)
        {
            throw new CustomBasicException("Deck out of range");
        }
        Deck = chosen;
        if (chosen <= 40)
        {
            //this is the gem cards.
            CardCategory = EnumCardCategory.Gem;
            if (chosen <= 6)
            {
                CardColor = EnumColors.Blue;
                if (chosen <= 4)
                {
                    FirstNumber = chosen;
                    return;
                }
                if (chosen == 5)
                {
                    FirstNumber = 5;
                    SecondNumber = 6;
                }
                else
                {
                    FirstNumber = 7;
                    SecondNumber = 8;
                }
                return;
            }
            if (chosen <= 12)
            {
                CardColor = EnumColors.Green;
                if (chosen == 7)
                {
                    FirstNumber = 1;
                    SecondNumber = 6;
                }
                if (chosen == 8)
                {
                    FirstNumber = 2;
                    SecondNumber = 5;
                }
                if (chosen == 9)
                {
                    FirstNumber = 3;
                }
                if (chosen == 10)
                {
                    FirstNumber = 4;
                }
                if (chosen == 11)
                {
                    FirstNumber = 7;
                }
                if (chosen == 12)
                {
                    FirstNumber = 8;
                }
                return;
            }
            if (chosen <= 18)
            {
                CardColor = EnumColors.DeepPink;
                if (chosen == 13)
                {
                    FirstNumber = 1;
                    SecondNumber = 2;
                    return;
                }
                if (chosen == 14)
                {
                    FirstNumber = 3;
                    SecondNumber = 4;
                    return;
                }
                FirstNumber = chosen - 10;
                return;
            }
            if (chosen <= 24)
            {
                CardColor = EnumColors.Yellow;
                if (chosen == 19)
                {
                    FirstNumber = 1;
                    SecondNumber = 3;
                }
                if (chosen == 20)
                {
                    FirstNumber = 2;
                }
                if (chosen == 21)
                {
                    FirstNumber = 4;
                }
                if (chosen == 22)
                {
                    FirstNumber = 5;
                }
                if (chosen == 23)
                {
                    FirstNumber = 6;
                    SecondNumber = 8;
                }
                if (chosen == 24)
                {
                    FirstNumber = 7;
                }
                return;
            }
            if (chosen <= 30)
            {
                CardColor = EnumColors.DarkOrange;
                if (chosen == 25)
                {
                    FirstNumber = 1;
                }
                if (chosen == 26)
                {
                    FirstNumber = 2;
                    SecondNumber = 4;
                }
                if (chosen == 27)
                {
                    FirstNumber = 3;
                }
                if (chosen == 28)
                {
                    FirstNumber = 5;
                    SecondNumber = 7;
                }
                if (chosen == 29)
                {
                    FirstNumber = 6;
                }
                if (chosen == 30)
                {
                    FirstNumber = 8;
                }
                return;
            }
            if (chosen <= 36)
            {
                CardColor = EnumColors.Purple;
                if (chosen == 31)
                {
                    FirstNumber = 1;
                }
                if (chosen == 32)
                {
                    FirstNumber = 2;
                }
                if (chosen == 33)
                {
                    FirstNumber = 3;
                    SecondNumber = 8;
                }
                if (chosen == 34)
                {
                    FirstNumber = 4;
                    SecondNumber = 7;
                }
                if (chosen == 35)
                {
                    FirstNumber = 5;
                }
                if (chosen == 36)
                {
                    FirstNumber = 6;
                }
                //if everything works, then tested up to 36
                return;
            }
            CardColor = EnumColors.Wild;
            if (chosen == 37)
            {
                FirstNumber = 1;
                SecondNumber = 8;
            }
            if (chosen == 38)
            {
                FirstNumber = 2;
                SecondNumber = 7;
            }
            if (chosen == 39)
            {
                FirstNumber = 3;
                SecondNumber = 6;
            }
            if (chosen == 40)
            {
                FirstNumber = 4;
                SecondNumber = 5;
            }
            return;
        }
        if (chosen <= 50)
        {
            CardCategory = EnumCardCategory.Faulty;
            return;
        }
        if (chosen <= 54)
        {
            CardCategory = EnumCardCategory.Jumper;
            return;
        }
        if (chosen <= 58)
        {
            CardCategory = EnumCardCategory.Safe;
            return;
        }
        CardCategory = EnumCardCategory.Fire; //the last card is what needs to be the last one in the deck though.
    }
    public void Reset()
    {
        //anything that is needed to reset.
    }

    int IComparable<YaBlewItCardInformation>.CompareTo(YaBlewItCardInformation? other)
    {
        if (CardColor != other!.CardColor)
        {
            return CardColor.CompareTo(other.CardColor);
        }
        if (FirstNumber != other.FirstNumber)
        {
            return FirstNumber.CompareTo(other.FirstNumber);
        }
        return Deck.CompareTo(other.Deck);
    }
}