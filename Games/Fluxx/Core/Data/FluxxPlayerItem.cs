namespace Fluxx.Core.Data;
[UseScoreboard]
public partial class FluxxPlayerItem : PlayerSingleHand<FluxxCardInformation>
{
    public DeckRegularDict<KeeperCard> KeeperList { get; set; } = new DeckRegularDict<KeeperCard>();
    public void UpdateKeepers()
    {
        NumberOfKeepers = KeeperList.Count;
        Bread = KeeperList.Any(items => items.Deck == EnumKeeper.Bread);
        Chocolate = KeeperList.Any(items => items.Deck == EnumKeeper.Chocolate);
        Cookies = KeeperList.Any(items => items.Deck == EnumKeeper.Cookies);
        Death = KeeperList.Any(items => items.Deck == EnumKeeper.Death);
        Dreams = KeeperList.Any(items => items.Deck == EnumKeeper.Dreams);
        Love = KeeperList.Any(items => items.Deck == EnumKeeper.Love);
        Milk = KeeperList.Any(items => items.Deck == EnumKeeper.Milk);
        Money = KeeperList.Any(items => items.Deck == EnumKeeper.Money);
        Peace = KeeperList.Any(items => items.Deck == EnumKeeper.Peace);
        Sleep = KeeperList.Any(items => items.Deck == EnumKeeper.Sleep);
        Television = KeeperList.Any(items => items.Deck == EnumKeeper.Television);
        TheBrain = KeeperList.Any(items => items.Deck == EnumKeeper.TheBrain);
        TheMoon = KeeperList.Any(items => items.Deck == EnumKeeper.TheMoon);
        TheRocket = KeeperList.Any(items => items.Deck == EnumKeeper.TheRocket);
        TheSun = KeeperList.Any(items => items.Deck == EnumKeeper.TheSun);
        TheToaster = KeeperList.Any(items => items.Deck == EnumKeeper.TheToaster);
        Time = KeeperList.Any(items => items.Deck == EnumKeeper.Time);
        War = KeeperList.Any(items => items.Deck == EnumKeeper.War);
    }
    [ScoreColumn]
    public int NumberOfKeepers { get; set; }
    [ScoreColumn]
    public bool Bread { get; set; }
    [ScoreColumn]
    public bool Chocolate { get; set; }
    [ScoreColumn]
    public bool Cookies { get; set; }
    [ScoreColumn]
    public bool Death { get; set; }
    [ScoreColumn]
    public bool Dreams { get; set; }
    [ScoreColumn]
    public bool Love { get; set; }
    [ScoreColumn]
    public bool Milk { get; set; }
    [ScoreColumn]
    public bool Money { get; set; }
    [ScoreColumn]
    public bool Peace { get; set; }
    [ScoreColumn]
    public bool Sleep { get; set; }
    [ScoreColumn]
    public bool Television { get; set; }
    [ScoreColumn]
    public bool TheBrain { get; set; }
    [ScoreColumn]
    public bool TheMoon { get; set; }
    [ScoreColumn]
    public bool TheRocket { get; set; }
    [ScoreColumn]
    public bool TheSun { get; set; }
    [ScoreColumn]
    public bool TheToaster { get; set; }
    [ScoreColumn]
    public bool Time { get; set; }
    [ScoreColumn]
    public bool War { get; set; }
    private FluxxGameContainer? _gameContainer;
    public bool ObeyedRulesWhenNotYourTurn()
    {
        if (_gameContainer == null)
        {
            _gameContainer = aa.Resolver!.Resolve<FluxxGameContainer>();
        }
        var tempList = _gameContainer.GetLimitList();
        if (tempList.Count > 2)
        {
            throw new CustomBasicException("Can only have 2 types of limits when its not your turn");
        }
        bool output = true; //has to be proven false.
        int count;
        tempList.ForEach(thisRule =>
        {
            count = thisRule.HowMany + _gameContainer.IncreaseAmount();
            switch (thisRule.Category)
            {
                case EnumRuleCategory.Hand:
                    if (MainHandList.Count > count)
                    {
                        output = false;
                    }
                    break;
                case EnumRuleCategory.Keeper:
                    if (KeeperList.Count > count)
                    {
                        output = false;
                    }
                    break;
                default:
                    throw new CustomBasicException("Can't find out whether its obeyed");
            }
        });
        return output;
    }
}