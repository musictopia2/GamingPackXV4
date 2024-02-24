namespace MonopolyCardGame.Core.Data;
public class CalculatorModel
{
    public EnumCardType Card { get; set; }
    public int Group { get; set; }
    public int HowMany { get; set; } //sometimes this is needed.
    public bool HasHotel { get; set; } //sometimes needed.
    public int Houses { get; set; } //may be 0.
    public decimal GetMoneyEarned(IListShuffler<MonopolyCardGameCardInformation> deckList)
    {
        if (Card == EnumCardType.None)
        {
            throw new CustomBasicException("Must have a card type in order to get the money earned");
        }
        if (Card == EnumCardType.IsRailRoad)
        {
            if (HowMany < 2 || HowMany > 4)
            {
                throw new CustomBasicException("Must have between 2 and 4 railroads");
            }
            if (HowMany == 2)
            {
                return 250;
            }
            if (HowMany == 3)
            {
                return 500;
            }
            if (HowMany == 4)
            {
                return 1000;
            }
            throw new CustomBasicException("Unknown for railroads");
        }
        if (Card == EnumCardType.IsUtilities)
        {
            return 500;
        }
        if (Card != EnumCardType.IsProperty)
        {
            throw new CustomBasicException("The last possibility in order to calculate the score is property");
        }
        var card = deckList.FirstOrDefault(x => x.Group == Group) ?? throw new CustomBasicException("Group not found when trying to calculate the property value");
        decimal moneyEarned = card.Money;
        if (Houses == 0 && HasHotel == false)
        {
            return moneyEarned;
        }
        decimal extras;
        if (HasHotel)
        {
            extras = TotalHouseAmount(moneyEarned, 4);
            extras += 500;
        }
        else
        {
            extras = TotalHouseAmount(moneyEarned, Houses);
        }
        moneyEarned += extras;
        return moneyEarned;
    }
    private static decimal TotalHouseAmount(decimal moneyEarned, int howMany)
    {
        if (howMany == 0 || howMany > 4)
        {
            throw new CustomBasicException("Must have between 1 and 4 housrs");
        }
        return moneyEarned * howMany;
    }
    public BasicList<MonopolyCardGameCardInformation> GetCards(IListShuffler<MonopolyCardGameCardInformation> deckList)
    {
        if (Card == EnumCardType.None)
        {
            throw new CustomBasicException("Must have a card type in order to get a list of cards needed");
        }
        BasicList<MonopolyCardGameCardInformation> output = [];
        BasicList<MonopolyCardGameCardInformation> list;
        MonopolyCardGameCardInformation card;
        if (Card == EnumCardType.IsProperty)
        {
            if (Group == 0)
            {
                throw new CustomBasicException("Must have a group id in order to get a card in this group");
            }
            list = deckList.Where(x => x.Group == Group).ToBasicList();
            output.AddRange(list.GetClonedCards());
            if (HasHotel)
            {
                card = deckList.Where(x => x.WhatCard == EnumCardType.IsHotel).Take(1).Single();
                output.Add(card.GetClonedCard());
                if (Houses > 0)
                {
                    throw new CustomBasicException("Cannot have both houses and hotels");
                }
                return output;
            }
            if (Houses == 0)
            {
                return output;
            }
            if (Houses < 0 || Houses > 4)
            {
                throw new CustomBasicException("Must have between 0 and 4 houses");
            }
            card = deckList.Where(x => x.WhatCard == EnumCardType.IsHouse && x.HouseNum == Houses).Take(1).Single();
            output.Add(card.GetClonedCard());
            return output;
        }
        if (Card == EnumCardType.IsUtilities)
        {
            list = deckList.Where(x => x.WhatCard == EnumCardType.IsUtilities).ToBasicList();
            output.AddRange(list.GetClonedCards());
            return output;
        }
        if (HowMany < 2)
        {
            throw new CustomBasicException("You must choose at least 2 railroads");
        }
        if (HowMany > 4)
        {
            throw new CustomBasicException("The maximum amount of railroads you can have is 4");
        }
        if (Card != EnumCardType.IsRailRoad)
        {
            throw new CustomBasicException("The last possibility is railroad");
        }
        list = deckList.Where(x => x.WhatCard == EnumCardType.IsRailRoad).Take(HowMany).ToBasicList();
        output.AddRange(list.GetClonedCards());
        return output;
    }
    
}