namespace DealCardGame.Core.Logic;
public static class PropertiesExtensions
{
    extension (DealCardGamePlayerItem player)
    {
        public void ClearPlayerProperties(EnumColor color)
        {
            var property = player.SetData.Single(x => x.Color == color);
            property.Cards.Clear();
        }
        public void ClonePlayerProperties(PrivateModel model)
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
        public bool WasHouseOrHotelBanked(DealCardGameCardInformation card)
        {
            return player.BankedCards.ObjectExist(card.Deck);
        }
        public int HowManyMonopolies => player.SetData.Count(x => x.HasRequiredSet);
        public void CloneSelectedPropertiesForPayments(PrivateModel model, DeckRegularDict<DealCardGameCardInformation> cardsChosen)
        {
            // Ensure model.SetData is initialized if not already
            model.SetData ??= [];

            foreach (var item in player.SetData)
            {
                // Check if the property already exists in the model's SetData
                var existingProperty = model.SetData.FirstOrDefault(x => x.Color == item.Color);

                if (existingProperty == null)
                {
                    // If the property doesn't exist, create a new one
                    SetPropertiesModel p = new();
                    p.Color = item.Color;

                    // Filter the cards to only include those present in cardsChosen
                    p.Cards = item.Cards
                        .Where(card => cardsChosen.Any(chosen => chosen.Deck == card.Deck))
                        .DistinctBy(card => card.Deck) // Ensure no duplicates based on the Deck property
                        .ToRegularDeckDict();

                    model.SetData.Add(p);
                }
                else
                {
                    // If the property exists, merge the cards while avoiding duplicates
                    var filteredCards = item.Cards
                        .Where(card => cardsChosen.Any(chosen => chosen.Deck == card.Deck))
                        .DistinctBy(card => card.Deck);

                    foreach (var card in filteredCards)
                    {
                        if (!existingProperty.Cards.ObjectExist(card.Deck))
                        {
                            existingProperty.Cards.Add(card); // Add only if the card is not already present
                        }
                    }
                }

            }

        }
        public void AddSingleCardToPlayerPropertySet(DealCardGameCardInformation card, EnumColor color)
        {
            var list = player.SetData.GetCards(color);
            list.Add(card);
        }
        public void AddSeveralCardsToPlayerPropertySet(DeckRegularDict<DealCardGameCardInformation> cards, EnumColor color)
        {
            var list = player.SetData.GetCards(color);
            list.AddRange(cards);
        }
    }
    extension (PrivateModel model)
    {
        public bool IsValidState
        {
            get
            {
                foreach (var item in model.SetData)
                {
                    bool hasHouse = item.Cards.Any(x => x.ActionCategory == EnumActionCategory.House);
                    bool hasHotel = item.Cards.Any(x => x.ActionCategory == EnumActionCategory.Hotel);
                    if (hasHotel == true && hasHouse == false)
                    {
                        return false; //because you need a house before you can have a hotel
                    }
                    if (hasHouse && item.HasRequiredSet == false)
                    {
                        return false; //because you need a set before you can have a house
                    }
                }
                return true;
            }
        }
        public PrivateModel CloneTemporaryModel()
        {
            PrivateModel tempModel = new();
            tempModel.NeedsPayment = model.NeedsPayment;
            foreach (var item in model.BankedCards)
            {
                tempModel.BankedCards.Add(item);
            }
            tempModel.SetData = [];
            foreach (var item in model.SetData)
            {
                SetPropertiesModel p = new();
                p.Color = item.Color;
                p.Cards = [];
                foreach (var card in item.Cards)
                {
                    p.Cards.Add(card);
                }
                tempModel.SetData.Add(p);
            }
            foreach (var item in model.Payments)
            {
                tempModel.Payments.Add(item);
            }

            return tempModel;
        }
    }
    extension (SetPropertiesModel property)
    {
        public bool HasRequiredHotel
        {
            get
            {
                if (property.Cards.Any(x => x.ActionCategory == EnumActionCategory.Hotel))
                {
                    return true;
                }
                return false;
            }
            
        }
        public bool HasRequiredHouse
        {
            get
            {
                if (property.Cards.Any(x => x.ActionCategory == EnumActionCategory.House))
                {
                    return true;
                }
                return false;
            }
            
        }
        public bool HasRequiredSet
        {
            get
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
        }
    }
    extension(BasicList<SetPropertiesModel> setData)
    {
        public SetPropertiesModel? GetPropertyFromCard(int deck)
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
        public DeckRegularDict<DealCardGameCardInformation> GetCards(EnumColor color)
        {
            return setData.Single(x => x.Color == color).Cards;
        }
    }
    extension (DealCardGamePlayerItem player)
    {
        public SetPropertiesModel? GetPropertyFromCard(int deck)
        {
            return player.SetData.GetPropertyFromCard(deck);
        }
    }
    extension (BasicList<SetPropertiesModel> properties)
    {
        public BasicList<SetPropertiesModel> GetSelectedProperties()
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
        public void RemoveCardFromPlayerSet(int deck, EnumColor color)
        {
            var list = properties.GetCards(color);
            list.RemoveObjectByDeck(deck);
        }
        public DeckRegularDict<DealCardGameCardInformation> GetAllCardsFromPlayersSet()
        {
            DeckRegularDict<DealCardGameCardInformation> output = [];
            foreach (var item in properties)
            {
                output.AddRange(item.Cards);
            }
            return output;
        }
    }
    extension (BasicList<DealCardGameCardInformation> list)
    {
        public bool HasHouse => list.Any(x => x.ActionCategory == EnumActionCategory.House);
        public bool HasHotel => list.Any(x => x.ActionCategory == EnumActionCategory.Hotel);
        public int RentForSet(EnumColor color, bool hasHouse, bool hasHotel)
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
        public int HowManyCompleted => list.Count(x => x.ActionCategory == EnumActionCategory.None);
    }
    extension (EnumColor color)
    {
        public bool CanUseHouseOrHotel
        {
            get
            {
                if (color == EnumColor.Black || color == EnumColor.Lime)
                {
                    return false;
                }
                return true;
            }
            
        }
        public int RentForGroup(int howManyInSet)
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
    }
    
    
    
    
    
    
    
}