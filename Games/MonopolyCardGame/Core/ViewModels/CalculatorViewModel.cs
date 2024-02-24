namespace MonopolyCardGame.Core.ViewModels;
public class CalculatorViewModel
{
    private BasicList<CalculatorModel> _list { get; set; } = [];
    public EnumCalculatorStatus Status { get; private set; }
    public int PropertyGroupChosen { get; private set; }
    public BasicList<MonopolyCardGameCardInformation> StartNewEntry(BasicList<MonopolyCardGameCardInformation> cardsOwned,
        IListShuffler<MonopolyCardGameCardInformation> deck
        )
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        PropertyGroupChosen = 0;
        Status = EnumCalculatorStatus.ChooseCardCategory;
        output.AddRange(GetPropertyCards(cardsOwned, deck));
        output.AddRange(GetSpecialCards(cardsOwned, deck));
        return output;
    }
    private BasicList<MonopolyCardGameCardInformation> GetPropertyCards(BasicList<MonopolyCardGameCardInformation> cardsOwned,
        IListShuffler<MonopolyCardGameCardInformation> deck)
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        var first = cardsOwned.Where(x => x.Group > 0).GroupBy(x => x.Group);
        foreach (var item in first)
        {
            if (_list.Any(x => x.Group == item.Key) == false)
            {
                var card = deck.First(x => x.Group == item.Key);
                output.Add(card.GetClonedCard());
            }
        }
        return output;
    }
    private BasicList<MonopolyCardGameCardInformation> GetSpecialCards(BasicList<MonopolyCardGameCardInformation> cardsOwned,
        IListShuffler<MonopolyCardGameCardInformation> deck)
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        BasicList<EnumCardType> categories = [EnumCardType.IsRailRoad, EnumCardType.IsUtilities];
        foreach (var item in categories)
        {
            bool rets;
            rets = cardsOwned.Any(x => x.WhatCard == item);
            if (rets == false)
            {
                continue;
            }
            if (_list.Any(x => x.Card == item))
            {
                continue;
            }
            var card = deck.First(x => x.WhatCard == item);
            output.Add(card);
        }
        return output;
    }
    public void Cancel()
    {
        PropertyGroupChosen = 0;
        Status = EnumCalculatorStatus.None; //not doing anything anymore.  means if you are in the middle and decide to cancel, start over again i think
    }
    public void ChooseUtilities()
    {
        CalculatorModel calculator = new()
        {
            Card = EnumCardType.IsUtilities
        };
        _list.Add(calculator);
        Cancel();
    }
    public void CompletelyClearCalculator()
    {
        _list.Clear(); //hopefully this simple.
        Cancel();
    }
    public void BackToMainCategory()
    {
        Cancel();
        Status = EnumCalculatorStatus.ChooseCardCategory;
    }
    public void ChooseRailroads()
    {
        Status = EnumCalculatorStatus.ChooseNumberOfRailroads;
    }
    public void ChooseProperties(int group)
    {
        PropertyGroupChosen = group;
        Status = EnumCalculatorStatus.ChoosePropertyInformation;
    }
    public void EnterRailroads(int howMany)
    {
        if (Status != EnumCalculatorStatus.ChooseNumberOfRailroads)
        {
            throw new CustomBasicException("The status must be choose number of railroads");
        }
        CheckRailroads(howMany);
        CalculatorModel calculator = new()
        {
            Card = EnumCardType.IsRailRoad,
            HowMany = howMany
        };
        _list.Add(calculator);
    }
    private static void CheckRailroads(int howMany)
    {
        if (howMany < 2 || howMany > 4)
        {
            throw new CustomBasicException("Must have between 2 and 4 railroads");
        }
    }
    public void EditRailroads(int howMany)
    {
        if (Status != EnumCalculatorStatus.EditRailroads)
        {
            throw new CustomBasicException("The status must be editing railroads");
        }
        CheckRailroads(howMany);
        CalculatorModel item = _list.Single(x => x.Card == EnumCardType.IsRailRoad);
        item.HowMany = howMany;
        Cancel(); //i think.
    }
    public void EditPropertyAlone()
    {
        PrivateEditProperty(0, false);
    }
    public void EditPropertyWithHotel()
    {
        PrivateEditProperty(0, true);
    }
    public void EditPropertyWithHouses(int howMany)
    {
        PrivateEditProperty(howMany, false);
    }
    public void EnterPropertyAlone()
    {
        PrivateEnterProperty(0, false);
    }
    public void EnterPropertyWithHotel()
    {
        PrivateEnterProperty(0, true);
    }
    public void EnterPropertyWithHouses(int howMany)
    {
        PrivateEnterProperty(howMany, false);
    }
    private void PrivateEditProperty(int houses, bool hasHotel)
    {
        if (Status != EnumCalculatorStatus.EditProperty)
        {
            throw new CustomBasicException("Must be edit property information");
        }
        CheckProperties(houses, hasHotel);
        CalculatorModel calculator = _list.Single(x => x.Group == PropertyGroupChosen);
        calculator.HasHotel = hasHotel;
        calculator.HowMany = houses;
        Cancel();
    }
    private void PrivateEnterProperty(int houses, bool hasHotel)
    {
        if (Status != EnumCalculatorStatus.ChoosePropertyInformation)
        {
            throw new CustomBasicException("Must be choose property information");
        }
        CheckProperties(houses, hasHotel);

        CalculatorModel calculator = new()
        {
            Card = EnumCardType.IsProperty
        };
        calculator.HasHotel = hasHotel;
        calculator.Houses = houses;
        _list.Add(calculator);
        Cancel();
    }
    private void CheckProperties(int houses, bool hasHotel)
    {
        if (PropertyGroupChosen < 1 || PropertyGroupChosen > 8)
        {
            throw new CustomBasicException("The property group must be between 1 and 8");
        }
        if (houses < 0 || houses > 4)
        {
            throw new CustomBasicException("Must have 0 to 4 houses");
        }
        if (houses > 0 && hasHotel)
        {
            throw new CustomBasicException("Cannot have both houses and hotels");
        }
    }
    public void StartEditing(CalculatorModel calculator) //the calculator has the function needed to get the proper list.
    {
        if (calculator.Card == EnumCardType.IsUtilities)
        {
            throw new CustomBasicException("Cannot edit utilities because only one choice");
        }
        if (calculator.Card == EnumCardType.IsRailRoad)
        {
            Status = EnumCalculatorStatus.EditRailroads;
            return;
        }
        if (calculator.Card == EnumCardType.IsProperty)
        {
            PropertyGroupChosen = calculator.Group;
            Status = EnumCalculatorStatus.EditProperty;
            return;
        }
        throw new CustomBasicException("Can only edit railroads and properties");
    }
    public void DeleteEntry(CalculatorModel calculator)
    {
        _list.RemoveSpecificItem(calculator);
    }
    public BasicList<CalculatorModel> GetTotalCalculations => _list.ToBasicList(); //so can't mess up the original list.
    
    //i do give the list back.
    //however, i do like the idea of the other 2 methods if i ever need autoresume for this.
    //on the other hand, maybe the viewmodel can figure out how to save if necessary (?)




}