namespace Fluxx.Core.Cards;
public class GoalCard : FluxxCardInformation
{
    public new EnumGoalRegular Deck
    {
        get
        {
            return (EnumGoalRegular)base.Deck;
        }
        set
        {
            base.Deck = (int)value;
        }
    }
    public GoalCard()
    {
        CardType = EnumCardType.Goal;
    }
    public int HowMany()
    {
        if (Deck == EnumGoalRegular.Keepers)
        {
            return 5;
        }
        if (Deck == EnumGoalRegular.CardsInHand)
        {
            return 10;
        }
        return 0;
    }
    public override bool IncreaseOne()
    {
        if (HowMany() > 0)
        {
            return true;
        }
        return base.IncreaseOne();
    }
    private EnumSpecialGoalSpecial SpecialGoal()
    {
        int count = HowMany();
        if (count == 5)
        {
            return EnumSpecialGoalSpecial.Keepers;
        }
        if (count == 10)
        {
            return EnumSpecialGoalSpecial.Hand;
        }
        return EnumSpecialGoalSpecial.None;
    }
    private static BasicList<Tuple<EnumKeeper, bool>> GetRegularGoals(EnumKeeper Keeper1, EnumKeeper Keeper2)
    {
        BasicList<Tuple<EnumKeeper, bool>> thisList = new()
        {
            new Tuple<EnumKeeper, bool>(Keeper1, false),
            new Tuple<EnumKeeper, bool>(Keeper2, false)
        };
        return thisList;
    }
    private Tuple<EnumSpecialGoalSpecial, BasicList<Tuple<EnumKeeper, bool>>>? _goal;
    public override void Populate(int Chosen)
    {
        Deck = (EnumGoalRegular)Chosen;
        PopulateDescription();
        PopulateGoal();
    }
    public void PopulateGoal()
    {
        _goal = PrivateGetGoalInfo();
    }
    private Tuple<EnumSpecialGoalSpecial, BasicList<Tuple<EnumKeeper, bool>>> PrivateGetGoalInfo()
    {
        BasicList<Tuple<EnumKeeper, bool>> thisList = new();
        EnumSpecialGoalSpecial thisSpecial;
        thisSpecial = SpecialGoal();
        switch (Deck)
        {
            case EnumGoalRegular.AllYouNeedIsLove:
                {
                    thisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.Love, false));
                    break;
                }
            case EnumGoalRegular.BakedGoods:
                {
                    thisList = GetRegularGoals(EnumKeeper.Bread, EnumKeeper.Cookies);
                    break;
                }
            case EnumGoalRegular.BedTime:
                {
                    thisList = GetRegularGoals(EnumKeeper.Sleep, EnumKeeper.Time);
                    break;
                }
            case EnumGoalRegular.ChocolateCookies:
                {
                    thisList = GetRegularGoals(EnumKeeper.Chocolate, EnumKeeper.Cookies);
                    break;
                }
            case EnumGoalRegular.ChocolateMilk:
                {
                    thisList = GetRegularGoals(EnumKeeper.Chocolate, EnumKeeper.Milk);
                    break;
                }
            case EnumGoalRegular.DeathByChocolate:
                {
                    thisList = GetRegularGoals(EnumKeeper.Death, EnumKeeper.Chocolate);
                    break;
                }
            case EnumGoalRegular.DreamLand:
                {
                    thisList = GetRegularGoals(EnumKeeper.Sleep, EnumKeeper.Dreams);
                    break;
                }
            case EnumGoalRegular.HeartsAndMinds:
                {
                    thisList = GetRegularGoals(EnumKeeper.Love, EnumKeeper.TheBrain);
                    break;
                }
            case EnumGoalRegular.Hippyism:
                {
                    thisList = GetRegularGoals(EnumKeeper.Peace, EnumKeeper.Love);
                    break;
                }
            case EnumGoalRegular.MilkAndCookies:
                {
                    thisList = GetRegularGoals(EnumKeeper.Milk, EnumKeeper.Cookies);
                    break;
                }
            case EnumGoalRegular.NightAndDay:
                {
                    thisList = GetRegularGoals(EnumKeeper.TheMoon, EnumKeeper.TheSun);
                    break;
                }
            case EnumGoalRegular.PeaceNoWar:
                {
                    thisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.Peace, false));
                    thisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.War, true));
                    break;
                }
            case EnumGoalRegular.RocketScience:
                {
                    thisList = GetRegularGoals(EnumKeeper.TheRocket, EnumKeeper.TheBrain);
                    break;
                }
            case EnumGoalRegular.RocketToTheMoon:
                {
                    thisList = GetRegularGoals(EnumKeeper.TheRocket, EnumKeeper.TheMoon);
                    break;
                }
            case EnumGoalRegular.SquishyChocolate:
                {
                    thisList = GetRegularGoals(EnumKeeper.Chocolate, EnumKeeper.TheSun);
                    break;
                }
            case EnumGoalRegular.TheAppliances:
                {
                    thisList = GetRegularGoals(EnumKeeper.Television, EnumKeeper.TheToaster);
                    break;
                }
            case EnumGoalRegular.TheBrainNoTV:
                {
                    thisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.TheBrain, false));
                    thisList.Add(new Tuple<EnumKeeper, bool>(EnumKeeper.Television, true));
                    break;
                }
            case EnumGoalRegular.TimeIsMoney:
                {
                    thisList = GetRegularGoals(EnumKeeper.Time, EnumKeeper.Money);
                    break;
                }
            case EnumGoalRegular.Toast:
                {
                    thisList = GetRegularGoals(EnumKeeper.Bread, EnumKeeper.TheToaster);
                    break;
                }
            case EnumGoalRegular.WinningTheLottery:
                {
                    thisList = GetRegularGoals(EnumKeeper.Dreams, EnumKeeper.Money);
                    break;
                }
            case EnumGoalRegular.WarDeath:
                {
                    thisList = GetRegularGoals(EnumKeeper.War, EnumKeeper.Death);
                    break;
                }
        }
        return new Tuple<EnumSpecialGoalSpecial, BasicList<Tuple<EnumKeeper, bool>>>(thisSpecial, thisList);
    }
    public int WhoWon(int HowManyExtra, FluxxGameContainer gameContainer)
    {
        if (_goal == null)
        {
            PopulateGoal();
        }
        int count;
        BasicList<FluxxPlayerItem> tempList;
        if (_goal!.Item1 != EnumSpecialGoalSpecial.None)
        {
            count = HowMany();
            count += HowManyExtra;
            if (_goal.Item1 == EnumSpecialGoalSpecial.Hand)
            {
                tempList = gameContainer.PlayerList!.Where(items => items.MainHandList.Count >= count).OrderByDescending(items => items.MainHandList.Count).Take(2).ToBasicList();
            }
            else
            {
                tempList = gameContainer.PlayerList!.Where(items => items.KeeperList.Count >= count).OrderByDescending(items => items.KeeperList.Count).Take(2).ToBasicList();
            }
            if (tempList.Count == 0)
            {
                return 0;
            }
            if (tempList.Count == 1)
            {
                return tempList.Single().Id;
            }
            if (_goal.Item1 == EnumSpecialGoalSpecial.Hand && tempList.First().MainHandList.Count == tempList.Last().MainHandList.Count)
            {
                return 0;
            }
            if (_goal.Item1 == EnumSpecialGoalSpecial.Keepers && tempList.First().KeeperList.Count == tempList.Last().KeeperList.Count)
            {
                return 0;
            }
            return tempList.First().Id;
        }
        if (_goal.Item2.Count == 0)
        {
            throw new CustomBasicException("Must have at least one goal since its not keepers or in hand");
        }
        if (_goal.Item2.Count > 2)
        {
            throw new CustomBasicException("Can have a maximum of 2 keepers to reach a goal");
        }
        FluxxPlayerItem thisPlayer;
        if (_goal.Item2.Count == 1)
        {
            if (Deck != EnumGoalRegular.AllYouNeedIsLove)
            {
                throw new CustomBasicException("Only all you need is love can have only one goal to win");
            }
            thisPlayer = gameContainer.PlayerList!.Where(items => items.KeeperList.Count == 1 && items.KeeperList.Single().Deck == EnumKeeper.Love).SingleOrDefault()!;
        }
        else
        {
            if (_goal.Item2.Last().Item2 == true)
            {
                if (gameContainer.PlayerList!.Any(tempPlayer => tempPlayer.KeeperList.Any(thisKeeper => thisKeeper.Deck == _goal.Item2.Last().Item1)))
                {
                    return 0;
                }
                thisPlayer = gameContainer.PlayerList!.Where(tempPlayer => tempPlayer.KeeperList.Any(thisKeeper => thisKeeper.Deck == _goal.Item2.First().Item1)).SingleOrDefault()!;
            }
            else
            {
                thisPlayer = gameContainer.PlayerList!.Where(tempPlayer => tempPlayer.KeeperList.Any
                    (thisKeeper => thisKeeper.Deck == _goal.Item2.First().Item1) && tempPlayer.KeeperList.Any
                    (thisKeeper => thisKeeper.Deck == _goal.Item2.Last().Item1)).SingleOrDefault()!;
            }
        }
        if (thisPlayer == null)
        {
            return 0;
        }
        return thisPlayer.Id;
    }
}
