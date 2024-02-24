namespace MonopolyCardGame.Core.Data;
public class CalculatorModel
{
    public EnumCardType Card { get; set; }
    public int Group { get; set; }
    public int HowMany { get; set; } //sometimes this is needed.
    public bool HasHotel { get; set; } //sometimes needed.
    public int Houses { get; set; } //may be 0.
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
            //this is for properties.
            if (Group == 0)
            {
                throw new CustomBasicException("Must have a group id in order to get a card in this group");
            }
            list = deckList.Where(x => x.Group == Group).ToBasicList();
            output.AddRange(GetClonedCards(list));
            if (HasHotel)
            {
                card = deckList.Where(x => x.WhatCard == EnumCardType.IsHotel).Take(1).Single();
                output.Add(GetClonedCard(card));
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
            output.Add(GetClonedCard(card));
            return output;
        }
        if (Card == EnumCardType.IsUtilities)
        {
            list = deckList.Where(x => x.WhatCard == EnumCardType.IsUtilities).ToBasicList();
            output.AddRange(GetClonedCards(list));
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
        output.AddRange(GetClonedCards(list));
        return output;
    }
    private static MonopolyCardGameCardInformation GetClonedCard(MonopolyCardGameCardInformation card)
    {
        MonopolyCardGameCardInformation output = new();
        output.Populate(card.Deck);
        return output;  
    }
    private static BasicList<MonopolyCardGameCardInformation> GetClonedCards(BasicList<MonopolyCardGameCardInformation> list)
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        foreach (var item in list)
        {
            MonopolyCardGameCardInformation cloned = new();
            cloned.Populate(item.Deck);
            output.Add(cloned);
        }
        return output;
    }
}