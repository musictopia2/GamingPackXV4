﻿namespace LifeBoardGame.Core.Logic;
public static class Extensions
{
    public static DeckRegularDict<T> GetCardsLeft<T>(this IDeckDict<T> thisList, PlayerCollection<LifeBoardGamePlayerItem> playerList) where T : LifeBaseCard
    {
        var newList = thisList.ToRegularDeckDict();
        playerList.ForEach(thisPlayer =>
        {
            thisPlayer.Hand.ForEach(thisCard =>
            {
                if (newList.ObjectExist(thisCard.Deck))
                {
                    newList.RemoveObjectByDeck(thisCard.Deck);
                }
            });
        });
        return newList;
    }
    public static DeckRegularDict<T> GetLoadedCards<T>(this IDeckDict<T> thisList, PlayerCollection<LifeBoardGamePlayerItem> playerList)
        where T : LifeBaseCard, new()
    {
        var tempList = thisList.GetCardsLeft(playerList);
        tempList.ForEach(thisCard => thisCard.IsUnknown = true);
        tempList.ShuffleList();
        return tempList;
    }
    public static Dictionary<int, decimal> GetSellingPrices(this BasicList<decimal> thisList)
    {
        if (thisList.Count != 10)
        {
            throw new CustomBasicException("Must have 10 items; not " + thisList.Count);
        }
        Dictionary<int, decimal> newList = new();
        foreach (var thisItem in thisList)
        {
            newList.Add(newList.Count + 1, thisItem);
        }
        return newList;
    }
    public static string GetColor(this EnumGender thisGender)
    {
        if (thisGender.Value == EnumGender.Boy.Value)
        {
            return cc1.Blue;
        }
        if (thisGender.Value == EnumGender.Girl.Value)
        {
            return cc1.DeepPink;
        }
        throw new Exception("Must be boy or girl for this");
    }
    public static DeckRegularDict<CareerInfo> GetCareerList(this LifeBoardGamePlayerItem thisPlayer)
    {
        var tempList = thisPlayer.Hand.Where(items => items.Deck <= 9).Select(items => items.Deck).ToBasicList();
        DeckRegularDict<CareerInfo> output = new();
        foreach (var thisItem in tempList)
        {
            output.Add(CardsModule.GetCareerCard(thisItem));
        }
        return output;
    }
    public static string GetHouseName(this LifeBoardGamePlayerItem thisPlayer)
    {
        HouseInfo thisCard;
        thisCard = (HouseInfo)thisPlayer.Hand.Where(items => items.Deck >= 10 && items.Deck <= 18).SingleOrDefault()!;
        if (thisCard == null || thisCard!.Deck == 0)
        {
            return "";
        }
        return thisCard.HouseCategory.ToString();
    }
    public static HouseInfo GetHouseCard(this LifeBoardGamePlayerItem thisPlayer)
    {
        HouseInfo thisCard;
        thisCard = (HouseInfo)thisPlayer.Hand.Where(items => items.Deck >= 10 && items.Deck <= 18).SingleOrDefault()!;
        return thisCard;
    }
    public static decimal NetIncome(this LifeBoardGamePlayerItem thisPlayer)
    {
        return thisPlayer.MoneyEarned - thisPlayer.Loans;
    }
    public static LifeBaseCard GetBaseHouseCard(this LifeBoardGamePlayerItem thisPlayer)
    {
        return (from Items in thisPlayer.Hand
                where Items.Deck >= 10 && Items.Deck <= 18
                select Items).SingleOrDefault()!;
    }
    public static decimal InsuranceCost(this LifeBoardGamePlayerItem thisPlayer)
    {
        HouseInfo thisCard;
        thisCard = thisPlayer.GetHouseCard();
        if (thisCard == null)
        {
            return 0;
        }
        return thisCard.InsuranceCost;
    }
    public static SalaryInfo GetSalaryCard(this LifeBoardGamePlayerItem thisPlayer)
    {
        SalaryInfo thisCard;
        thisCard = (SalaryInfo)thisPlayer.Hand.Where(items => items.Deck >= 19 && items.Deck <= 27).SingleOrDefault()!;
        return thisCard;
    }
    public static BasicList<string> GetSalaryList(this BasicList<LifeBoardGamePlayerItem> tempList)
    {
        BasicList<string> output = new();
        tempList.ForConditionalItems(items => items.Salary > 0, thisPlayer => output.Add(thisPlayer.NickName));
        return output;
    }
    public static DeckRegularDict<SalaryInfo> GetSalaryList(this BasicList<int> tempList, PlayerCollection<LifeBoardGamePlayerItem> playerList)
    {
        DeckRegularDict<SalaryInfo> newList = new();
        foreach (var thisItem in tempList)
        {
            if (thisItem.SomeoneHasCard(playerList) == false)
            {
                newList.Add(CardsModule.GetSalaryCard(thisItem));
                newList.Last().IsUnknown = true;
            }
        }
        return newList;
    }
    public static decimal TaxesDue(this LifeBoardGamePlayerItem thisPlayer)
    {
        try
        {
            SalaryInfo thisCard;
            thisCard = (SalaryInfo)thisPlayer.Hand.Where(items => items.Deck >= 19 && items.Deck <= 27).SingleOrDefault()!;
            return thisCard.TaxesDue;
        }
        catch (Exception)
        {
            throw new CustomBasicException("Must have a salary to pay taxes");
        }
    }
    public static bool SomeoneHasCard(this int deck, PlayerCollection<LifeBoardGamePlayerItem> playerList)
    {
        foreach (var thisPlayer in playerList)
        {
            if (thisPlayer.Hand.ObjectExist(deck) == true)
            {
                return true;
            }
        }
        return false;
    }
    public static int PlayerHasCard(this int deck, PlayerCollection<LifeBoardGamePlayerItem> playerList)
    {
        foreach (var thisPlayer in playerList)
        {
            if (thisPlayer.Hand.ObjectExist(deck) == true)
            {
                return thisPlayer.Id;
            }
        }
        return 0;
    }
    public static DeckRegularDict<HouseInfo> GetHouseList(this BasicList<int> tempList, PlayerCollection<LifeBoardGamePlayerItem> playerList)
    {
        DeckRegularDict<HouseInfo> newList = new();
        foreach (var thisItem in tempList)
        {
            if (thisItem.SomeoneHasCard(playerList) == false)
            {
                newList.Add(CardsModule.GetHouseCard(thisItem));
                if (newList.Count == 2)
                {
                    return newList;
                }
            }
        }
        throw new CustomBasicException("Has to find 2 cards to choose from for house");
    }
    public static LifeBaseCard GetStockCard(this LifeBoardGamePlayerItem thisPlayer, int stockNumber)
    {
        int deck;
        deck = 27 + stockNumber;
        return (from items in thisPlayer.Hand
                where items.Deck == deck
                select items).Single();
    }
}