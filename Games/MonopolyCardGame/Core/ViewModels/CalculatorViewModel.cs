namespace MonopolyCardGame.Core.ViewModels;
public class CalculatorViewModel
{
    private readonly BasicList<CalculatorModel> _list = [];
    public EnumCalculatorStatus Status { get; private set; }
    public int PropertyGroupChosen { get; private set; }
    public void ClearCalculator()
    {
        _list.Clear();
    }
    public void RestoreCalculator(PrivateModel model)
    {
        _list.Clear();
        _list.AddRange(model.Calculations);
    }
    private IListShuffler<MonopolyCardGameCardInformation>? _deck;
    public BasicList<MonopolyCardGameCardInformation> StartNewEntry(
        IListShuffler<MonopolyCardGameCardInformation> deck
        )
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        PropertyGroupChosen = 0;
        Status = EnumCalculatorStatus.ChooseCardCategory;
        output.AddRange(GetPropertyCards(deck));
        output.AddRange(GetSpecialCards(deck));
        foreach (var item in output)
        {
            item.PlainCategory = EnumPlainCategory.Chooser;
        }
        _deck = deck;
        return output;
    }
    private static BasicList<MonopolyCardGameCardInformation> GetPropertyCards(
        IListShuffler<MonopolyCardGameCardInformation> deck)
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        var first = deck.Where(x => x.Group > 0).GroupBy(x => x.Group);
        foreach (var item in first)
        {
            var card = item.First();
            output.Add(card.GetClonedCard());
        }
        //var first = cardsOwned.Where(x => x.Group > 0).GroupBy(x => x.Group);
        //foreach (var item in first)
        //{
        //    var card = deck.First(x => x.Group == item.Key);
        //    output.Add(card.GetClonedCard());
        //}
        return output;
    }
    private static BasicList<MonopolyCardGameCardInformation> GetSpecialCards(
        IListShuffler<MonopolyCardGameCardInformation> deck)
    {
        BasicList<MonopolyCardGameCardInformation> output = [];
        BasicList<EnumCardType> categories = [EnumCardType.IsRailRoad, EnumCardType.IsUtilities];
        foreach (var item in categories)
        {
            //bool rets;
            //rets = cardsOwned.Any(x => x.WhatCard == item);
            //if (rets == false)
            //{
            //    continue;
            //}
            var card = deck.First(x => x.WhatCard == item);
            output.Add(card.GetClonedCard());
        }
        return output;
    }
    public void Cancel()
    {
        PropertyGroupChosen = 0;
        Status = EnumCalculatorStatus.None; //not doing anything anymore.  means if you are in the middle and decide to cancel, start over again i think
        StateHasChanged?.Invoke();
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
    public Action? StateHasChanged { get; set; }
    private MonopolyCardGameCardInformation? _railroad;
    public MonopolyCardGameCardInformation ChooseRailroads()
    {
        Status = EnumCalculatorStatus.ChooseNumberOfRailroads;
        if (_deck is null)
        {
            throw new CustomBasicException("Did not initialize the deck");
        }
        if (_railroad is not null)
        {
            return _railroad;
        }
        MonopolyCardGameCardInformation card = _deck.First(x => x.WhatCard == EnumCardType.IsRailRoad);
        _railroad = card.GetClonedCard();
        _railroad.PlainCategory = EnumPlainCategory.Chooser;
        return _railroad;
    }
    public BasicList<MonopolyCardGameCardInformation> ChooseProperties(int group)
    {
        if (group == 0)
        {
            throw new CustomBasicException("Must specify a group");
        }
        BasicList<MonopolyCardGameCardInformation> output = [];
        PropertyGroupChosen = group;
        Status = EnumCalculatorStatus.ChoosePropertyInformation;
        if (_deck is null)
        {
            throw new CustomBasicException("Did not initialize the deck");
        }
        MonopolyCardGameCardInformation card;
        card = _deck.First(x => x.Group == group);
        output.Add(card.GetClonedCard());
        4.Times(y =>
        {
            card = _deck.First(x => x.WhatCard == EnumCardType.IsHouse && x.HouseNum == y);
            output.Add(card.GetClonedCard());
        });
        card = _deck.First(x => x.WhatCard == EnumCardType.IsHotel);
        output.Add(card.GetClonedCard());
        foreach (var item in output)
        {
            item.PlainCategory = EnumPlainCategory.Chooser;
        }
        return output;
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
        Cancel();
    }
    private static void CheckRailroads(int howMany)
    {
        if (howMany < 2 || howMany > 4)
        {
            throw new CustomBasicException("Must have between 2 and 4 railroads");
        }
    }
    public void EnterPropertyFromCard(MonopolyCardGameCardInformation card)
    {
        if (card.WhatCard == EnumCardType.IsHotel)
        {
            EnterPropertyWithHotel();
            return;
        }
        if (card.WhatCard == EnumCardType.IsProperty)
        {
            EnterPropertyAlone();
            return;
        }
        if (card.WhatCard == EnumCardType.IsHouse)
        {
            EnterPropertyWithHouses(card.HouseNum);
            return;
        }
        throw new CustomBasicException("Failed to enter property from card");
    }
    private void EnterPropertyAlone()
    {
        PrivateEnterProperty(0, false);
    }
    private void EnterPropertyWithHotel()
    {
        PrivateEnterProperty(0, true);
    }
    private void EnterPropertyWithHouses(int howMany)
    {
        PrivateEnterProperty(howMany, false);
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
        calculator.Group = PropertyGroupChosen;
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
    public void DeleteEntry(CalculatorModel calculator)
    {
        _list.RemoveSpecificItem(calculator);
    }
    public BasicList<CalculatorModel> GetTotalCalculations => _list.ToBasicList(); //so can't mess up the original list.
    
    //i do give the list back.
    //however, i do like the idea of the other 2 methods if i ever need autoresume for this.
    //on the other hand, maybe the viewmodel can figure out how to save if necessary (?)
}