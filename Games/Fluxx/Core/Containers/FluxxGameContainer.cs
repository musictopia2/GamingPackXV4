namespace Fluxx.Core.Containers;
[SingletonGame]
[AutoReset]
public class FluxxGameContainer : CardGameContainer<FluxxCardInformation, FluxxPlayerItem, FluxxSaveInfo>
{
    public FluxxGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<FluxxCardInformation> deckList,
        IRandomGenerator random)
        : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
        DescriptionList = Resources.FluxxDescriptions.GetResource<BasicList<string>>();
    }
    public int OtherTurn
    {
        get
        {
            return SaveRoot!.PlayOrder.OtherTurn;
        }
        set
        {
            SaveRoot!.PlayOrder.OtherTurn = value;
        }
    }
    public int WhoWonGame()
    {
        if (Test!.ImmediatelyEndGame)
        {
            return 1; //let the first player win if we need to quickly test new game naturally.
        }
        if (SaveRoot!.GoalList.Count > 2)
        {
            return 0;
        }
        int extras = IncreaseAmount();
        var tempList = SaveRoot.GoalList.Select(items => items.WhoWon(extras, this)).ToBasicList();
        tempList.RemoveAllOnly(x => x == 0);
        if (tempList.Count == 0)
        {
            return 0;
        }
        if (tempList.Count == 2)
        {
            return 0;
        }
        int index = tempList.Single();
        if (index == 0)
        {
            return 0;
        }
        if (index == WhoTurn)
        {
            return index;
        }
        var thisPlayer = PlayerList![index];
        if (thisPlayer.ObeyedRulesWhenNotYourTurn() == false)
        {
            return 0;
        }
        return index;
    }
    internal Func<Task>? LoadPlayAsync { get; set; }
    internal Func<Task>? LoadGiveAsync { get; set; }
    static internal BasicList<string> DescriptionList { get; set; } = new();
    public DeckRegularDict<FluxxCardInformation> QuePlayList { get; set; } = new DeckRegularDict<FluxxCardInformation>();
    internal bool DoDrawTemporary { get; set; } = false;
    internal bool AllNewCards { get; set; }
    public ActionCard? CurrentAction { get; set; }
    public int IncreaseAmount()
    {
        if (SaveRoot!.RuleList.Any(items => items.EverythingByOne()))
        {
            return 1;
        }
        return 0;
    }
    public void RemoveLimits()
    {
        SaveRoot!.RuleList.RemoveAllAndObtain(items => items.Category == EnumRuleCategory.Hand || items.Category == EnumRuleCategory.Keeper); //hopefully that works too
    }
    public DeckRegularDict<RuleCard> GetLimitList()
    {
        return SaveRoot!.RuleList.Where(items => items.Category == EnumRuleCategory.Hand || items.Category == EnumRuleCategory.Keeper).ToRegularDeckDict();
    }
    public bool HasDoubleAgenda() => SaveRoot!.RuleList.Any(items => items.Deck == EnumRuleText.DoubleAgenda);
    private bool PlayerHasKeepers(bool most)
    {
        BasicList<FluxxPlayerItem> tempList;
        if (most)
        {
            tempList = PlayerList!.OrderByDescending(items => items.KeeperList.Count).Take(2).ToBasicList();
        }
        else
        {
            tempList = PlayerList!.OrderBy(items => items.KeeperList.Count).Take(2).ToBasicList();
        }
        if (tempList.First().KeeperList.Count == tempList.Last().KeeperList.Count)
        {
            return false;
        }
        int tempTurn = SingleInfo!.Id;
        int index = tempList.First().Id;
        return index == tempTurn;
    }
    public bool HasFewestKeepers() => PlayerHasKeepers(false);
    public bool HasMostKeepers() => PlayerHasKeepers(true);
    public static BasicList<string> GetSortedKeeperList()
    {
        BasicList<int> firstList = Enumerable.Range(23, 18).ToBasicList();
        BasicList<string> output = new();
        firstList.ForEach(thisIndex =>
        {
            EnumKeeper thisKeep = thisIndex.ToEnum<EnumKeeper>();
            output.Add(thisKeep.ToString().TextWithSpaces());
        });
        output.Sort();
        return output;
    }
    public BasicList<int> TempActionHandList
    {
        get
        {
            return SaveRoot!.SavedActionData.TempHandList;
        }
        set
        {
            SaveRoot!.SavedActionData.TempHandList = value;
        }
    }
    public BasicList<PreviousCard> EverybodyGetsOneList
    {
        get
        {
            return SaveRoot!.SavedActionData.PreviousList;
        }
        set
        {
            SaveRoot!.SavedActionData.PreviousList = value;
        }
    }
    public bool IsFirstPlayRandom()
    {
        if (SaveRoot!.CardsPlayed > 0)
        {
            return false;
        }
        if (SaveRoot.PlayLimit == 1)
        {
            return false;
        }
        if (SingleInfo!.MainHandList.Count == 0)
        {
            return false; //because the main player has 0 cards
        }
        return SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.FirstPlayRandom);
    }
    public bool NeedsToRemoveGoal()
    {
        if (SaveRoot!.GoalList.Count == 3)
        {
            return true;
        }
        if (SaveRoot.GoalList.Count > 3)
        {
            throw new CustomBasicException("Too many goals");
        }
        return SaveRoot.GoalList.Count == 2 && HasDoubleAgenda() == false;
    }
    internal EnumActionScreen ScreenToLoad(int otherTurn)
    {
        if (otherTurn > 0)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
        }
        if (IsFirstPlayRandom())
        {
            if (otherTurn > 0)
            {
                SingleInfo = PlayerList![otherTurn];
            }
            return EnumActionScreen.ActionScreen;
        }
        if (otherTurn > 0)
        {
            SingleInfo = PlayerList![otherTurn];
        }
        if (otherTurn > 0 && SingleInfo!.ObeyedRulesWhenNotYourTurn() == false)
        {
            return EnumActionScreen.None; //because somebody needs to remove the necessary cards before it can be continued
        }
        if (CurrentAction == null)
        {
            return EnumActionScreen.None;
        }
        if (CurrentAction.Category == EnumActionScreen.OtherPlayer)
        {
            return EnumActionScreen.None;
        }
        return CurrentAction.Category;
    }
    public void RemoveFromHandOnly(FluxxCardInformation thisCard)
    {
        SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
    }

    public async Task DiscardRuleAsync(RuleCard thisRule)
    {
        if (AnimatePlayAsync == null)
        {
            throw new CustomBasicException("When discarding rules, nobody was animating play.  Rethink");
        }
        SaveRoot!.RuleList.RemoveObjectByDeck((int)thisRule.Deck);
        await AnimatePlayAsync.Invoke(thisRule);
    }
    public void RefreshRules()
    {
        SaveRoot!.PlayOrder.IsReversed = SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.ReverseOrder);
    }
}