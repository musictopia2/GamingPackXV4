namespace ClueCardGame.Core.Cards;
public class ClueCardGameCardInformation : SimpleDeckObject, IDeckObject, IComparable<ClueCardGameCardInformation>
{
    public EnumCardType WhatType { get; set; }
    public EnumCardValues CardValue { get; set; }
    public string Name { get; set; } = "";
    private static BasicList<string> __names = ["Mrs. Peacock", 
        "Mr. Green", "Professor Plum", "Miss Scarlet",
        "Mrs. White", "Colonel Mustard",
        "Candlestick", "Knife",
        "Rope", "Lead Pipe",
        "Ball Room", "Library", "Study",
        "Hall", "Dining Room"
        ];
    public ClueCardGameCardInformation()
    {
        DefaultSize = new SizeF(55, 72); //this is neeeded too.
    }
    public void Populate(int chosen)
    {
        //populating the card.
        Deck = chosen;
        if (chosen < 1 || chosen > 15)
        {
            throw new CustomBasicException("Only has 15 cards total");
        }
        switch (chosen)
        {
            case int _ when chosen < 7:
                {
                    WhatType = EnumCardType.IsCharacter;
                    break;
                }

            case int _ when chosen < 11:
                {
                    WhatType = EnumCardType.IsWeapon;
                    break;
                }

            default:
                {
                    WhatType = EnumCardType.IsRoom;
                    break;
                }
        }
        CardValue = chosen.ToEnum<EnumCardValues>();
        Name = __names[chosen - 1];
    }
    public override string GetKey()
    {
        return Deck.ToString();
    }
    public void Reset()
    {
        //anything that is needed to reset.
    }

    int IComparable<ClueCardGameCardInformation>.CompareTo(ClueCardGameCardInformation? other)
    {
        return CardValue.CompareTo(other!.CardValue);
    }
}