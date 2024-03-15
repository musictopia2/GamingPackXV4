namespace DealCardGame.Core.Logic;
public static class PropertiesExtensions
{
    public static void ClearPlayerProperties(this DealCardGamePlayerItem player, EnumColor color)
    {
        var property = player.SetData.Single(x => x.Color == color);
        property.Cards.Clear();
    }
    public static void ClonePlayerProperties(this DealCardGamePlayerItem player, PrivateModel model)
    {
        model.SetData = [];
        foreach (var item in player.SetData)
        {
            SetPropertiesModel p = new();
            p.Color = item.Color;
            p.Cards = item.Cards.ToRegularDeckDict();
            model.SetData.Add(p);
        }
    }
    public static bool HasRequiredSet(this SetPropertiesModel property)
    {
        var count = property.Cards.Count(x => x.ActionCategory == EnumActionCategory.None);
        if (count > 3)
        {
            return true; //no matter what.
        }
        if (property.Color == EnumColor.Black)
        {
            return false; //since black requires 4
        }
        if (property.Color == EnumColor.Lime || property.Color == EnumColor.DarkBlue || property.Color == EnumColor.Brown)
        {
            return count >= 2;
        }
        return count >= 3;
    }
    public static SetPropertiesModel? GetPropertyFromCard(this BasicList<SetPropertiesModel> setData, int deck)
    {
        foreach (var item in setData)
        {
            if (item.Cards.ObjectExist(deck))
            {
                return item;
            }
        }
        return null;
    }
    public static SetPropertiesModel? GetPropertyFromCard(this DealCardGamePlayerItem player, int deck)
    {
        return player.SetData.GetPropertyFromCard(deck);
    }
    public static BasicList<SetPropertiesModel> GetSelectedProperties(this BasicList<SetPropertiesModel> properties)
    {
        BasicList<SetPropertiesModel> output = [];
        foreach (var item in properties)
        {
            var list = item.Cards.GetSelectedItems();
            if (list.Count > 0)
            {
                SetPropertiesModel other = new()
                {
                    Cards = list,
                    Color = item.Color,
                };
                output.Add(other);
            }
        }
        return output;
    }
    public static void RemoveCardFromPlayerSet(this BasicList<SetPropertiesModel> properties, int deck, EnumColor color)
    {
        var list = properties.GetCards(color);
        list.RemoveObjectByDeck(deck);
    }
    public static DeckRegularDict<DealCardGameCardInformation> GetAllCardsFromPlayersSet(this BasicList<SetPropertiesModel> properties)
    {
        DeckRegularDict<DealCardGameCardInformation> output = [];
        foreach (var item in properties)
        {
            output.AddRange(item.Cards);
        }
        return output;
    }
    public static void AddSingleCardToPlayerPropertySet(this DealCardGamePlayerItem player, DealCardGameCardInformation card, EnumColor color)
    {
        var list = player.SetData.GetCards(color);
        list.Add(card);
    }
    public static void AddSeveralCardsToPlayerPropertySet(this DealCardGamePlayerItem player, DeckRegularDict<DealCardGameCardInformation> cards, EnumColor color)
    {
        var list = player.SetData.GetCards(color);
        list.AddRange(cards);
    }
    public static DeckRegularDict<DealCardGameCardInformation> GetCards(this BasicList<SetPropertiesModel> properties, EnumColor color)
    {
        return properties.Single(x => x.Color == color).Cards;
    }
    public static bool HasHouse(this BasicList<DealCardGameCardInformation> list) => list.Any(x => x.ActionCategory == EnumActionCategory.House);
    public static bool HasHotel(this BasicList<DealCardGameCardInformation> list) => list.Any(x => x.ActionCategory == EnumActionCategory.Hotel);
    public static int RentForSet(this BasicList<DealCardGameCardInformation> list, EnumColor color, bool hasHouse, bool hasHotel)
    {
        if (list.Count == 0)
        {
            return 0;
        }
        int count = list.Count(x => x.ActionCategory == EnumActionCategory.None);
        if (count == 0)
        {
            throw new CustomBasicException("Should had returned 0 items because had no list items");
        }
        int output = color.RentForGroup(count);
        if (hasHouse)
        {
            output += 3;
        }
        if (hasHotel)
        {
            output += 4;
        }
        return output;
    }
    public static int HowManyCompleted(this BasicList<DealCardGameCardInformation> list) => list.Count(x => x.ActionCategory == EnumActionCategory.None);
    public static bool CanUseHouseOrHotel(this EnumColor color)
    {
        if (color == EnumColor.Black || color == EnumColor.Lime)
        {
            return false;
        }
        return true;
    }
    public static int RentForGroup(this EnumColor color, int howManyInSet)
    {
        //this will not consider house or hotel.
        if (color == EnumColor.Brown)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet >= 2)
            {
                return 2;
            }
            throw new CustomBasicException("Failed to calculate rent for brown");
        }
        if (color == EnumColor.Cyan)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            if (howManyInSet >= 3)
            {
                return 3;
            }
            throw new CustomBasicException("Failed to calculate rent for cyan");
        }
        if (color == EnumColor.MediumVioletRed)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            if (howManyInSet >= 3)
            {
                return 4;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.DarkOrange)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 3;
            }
            if (howManyInSet >= 3)
            {
                return 5;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Red)
        {
            if (howManyInSet == 1)
            {
                return 2;
            }
            if (howManyInSet == 2)
            {
                return 3;
            }
            if (howManyInSet >= 3)
            {
                return 6;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Yellow)
        {
            if (howManyInSet == 1)
            {
                return 2;
            }
            if (howManyInSet == 2)
            {
                return 4;
            }
            if (howManyInSet >= 3)
            {
                return 6;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Green)
        {
            if (howManyInSet == 1)
            {
                return 2;
            }
            if (howManyInSet == 2)
            {
                return 4;
            }
            if (howManyInSet >= 3)
            {
                return 7;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.DarkBlue)
        {
            if (howManyInSet == 1)
            {
                return 3;
            }
            if (howManyInSet >= 2)
            {
                return 8;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Lime)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet >= 2)
            {
                return 2;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Black)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            if (howManyInSet == 3)
            {
                return 3;
            }
            if (howManyInSet >= 4)
            {
                return 4;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        throw new CustomBasicException("Unable to calculate rent");
    }
    public static int HowManyMonopolies(this DealCardGamePlayerItem player)
    {
        return player.SetData.Count(x => x.HasRequiredSet());
    }
}