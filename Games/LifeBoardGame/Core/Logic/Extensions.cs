namespace LifeBoardGame.Core.Logic;
public static class Extensions
{
    extension <T>(IDeckDict<T> list)
        where T: LifeBaseCard
    {
        public DeckRegularDict<T> GetCardsLeft(PlayerCollection<LifeBoardGamePlayerItem> playerList)
        {
            var newList = list.ToRegularDeckDict();
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
        public DeckRegularDict<T> GetLoadedCards(PlayerCollection<LifeBoardGamePlayerItem> playerList)
        {
            var tempList = list.GetCardsLeft(playerList);
            tempList.ForEach(thisCard => thisCard.IsUnknown = true);
            tempList.ShuffleList();
            return tempList;
        }
    }
    extension (BasicList<decimal> list)
    {
        public Dictionary<int, decimal> SellingPrices
        {
            get
            {
                if (list.Count != 10)
                {
                    throw new CustomBasicException("Must have 10 items; not " + list.Count);
                }
                Dictionary<int, decimal> newList = [];
                foreach (var thisItem in list)
                {
                    newList.Add(newList.Count + 1, thisItem);
                }
                return newList;
            }
            
        }
    }
    extension (EnumGender gender)
    {
        public string GetColor
        {
            get
            {
                if (gender.Value == EnumGender.Boy.Value)
                {
                    return cc1.Blue;
                }
                if (gender.Value == EnumGender.Girl.Value)
                {
                    return cc1.DeepPink;
                }
                throw new CustomBasicException("Must be boy or girl for this");
            }
            
        }
    }
    extension (LifeBoardGamePlayerItem player)
    {
        public DeckRegularDict<CareerInfo> GetCareerList()
        {
            var tempList = player.Hand.Where(items => items.Deck <= 9).Select(items => items.Deck).ToBasicList();
            DeckRegularDict<CareerInfo> output = new();
            foreach (var thisItem in tempList)
            {
                output.Add(CardsModule.GetCareerCard(thisItem));
            }
            return output;
        }
        public string HouseName
        {
            get
            {
                HouseInfo thisCard;
                thisCard = (HouseInfo)player.Hand.Where(items => items.Deck >= 10 && items.Deck <= 18).SingleOrDefault()!;
                if (thisCard == null || thisCard!.Deck == 0)
                {
                    return "";
                }
                return thisCard.HouseCategory.ToString();
            }   
        }
        public HouseInfo HouseCard
        {
            get
            {
                HouseInfo thisCard;
                thisCard = (HouseInfo)player.Hand.Where(items => items.Deck >= 10 && items.Deck <= 18).SingleOrDefault()!;
                return thisCard;
            }
        }
        public decimal NetIncome => player.MoneyEarned - player.Loans;
        public LifeBaseCard GetBaseHouseCard()
        {
            return (from Items in player.Hand
                    where Items.Deck >= 10 && Items.Deck <= 18
                    select Items).SingleOrDefault()!;
        }
        public decimal InsuranceCost
        {
            get
            {
                HouseInfo thisCard;
                thisCard = player.HouseCard;
                if (thisCard == null)
                {
                    return 0;
                }
                return thisCard.InsuranceCost;
            }
           
        }
        public SalaryInfo GetSalaryCard()
        {
            SalaryInfo thisCard;
            thisCard = (SalaryInfo)player.Hand.Where(items => items.Deck >= 19 && items.Deck <= 27).SingleOrDefault()!;
            return thisCard;
        }
        public decimal TaxesDue
        {
            get
            {
                try
                {
                    SalaryInfo thisCard;
                    thisCard = (SalaryInfo)player.Hand.Where(items => items.Deck >= 19 && items.Deck <= 27).SingleOrDefault()!;
                    return thisCard.TaxesDue;
                }
                catch (Exception)
                {
                    throw new CustomBasicException("Must have a salary to pay taxes");
                }
            }   
        }
        public LifeBaseCard GetStockCard(int stockNumber)
        {
            int deck;
            deck = 27 + stockNumber;
            return (from items in player.Hand
                    where items.Deck == deck
                    select items).Single();
        }
    }
    extension (BasicList<LifeBoardGamePlayerItem> list)
    {
        public BasicList<string> GetSalaryList()
        {
            BasicList<string> output = [];
            list.ForConditionalItems(items => items.Salary > 0, thisPlayer => output.Add(thisPlayer.NickName));
            return output;
        }
    }
    extension (BasicList<int> list)
    {
        public DeckRegularDict<SalaryInfo> GetSalaryList(PlayerCollection<LifeBoardGamePlayerItem> playerList)
        {
            DeckRegularDict<SalaryInfo> newList = new();
            foreach (var thisItem in list)
            {
                if (thisItem.SomeoneHasCard(playerList) == false)
                {
                    newList.Add(CardsModule.GetSalaryCard(thisItem));
                    newList.Last().IsUnknown = true;
                }
            }
            return newList;
        }
    }
    extension (int deck)
    {
        public int PlayerHasCard(PlayerCollection<LifeBoardGamePlayerItem> playerList)
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
        public bool SomeoneHasCard(PlayerCollection<LifeBoardGamePlayerItem> playerList)
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
    }
    extension(BasicList<int> list)
    {
        public DeckRegularDict<HouseInfo> GetHouseList(PlayerCollection<LifeBoardGamePlayerItem> playerList)
        {
            DeckRegularDict<HouseInfo> newList = new();
            foreach (var thisItem in list)
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
    }
    
    
    
}