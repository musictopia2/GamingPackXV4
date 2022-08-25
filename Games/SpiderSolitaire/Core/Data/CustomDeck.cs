namespace SpiderSolitaire.Core.Data;
public class CustomDeck : IRegularDeckInfo
{
    private readonly LevelClass _level;
    public CustomDeck(LevelClass level)
    {
        _level = level;
    }
    int IRegularDeckInfo.HowManyDecks
    {
        get
        {
            if (_level.LevelChosen == 3)
            {
                return 2;
            }
            if (_level.LevelChosen == 2)
            {
                return 4;
            }
            if (_level.LevelChosen == 1)
            {
                return 8;
            }
            throw new CustomBasicException("Needs levels 1, 2, or 3 for figuring out how many decks");
        }
    }
    bool IRegularDeckInfo.UseJokers => false;
    int IRegularDeckInfo.GetExtraJokers => 0;
    int IRegularDeckInfo.LowestNumber => 1;
    int IRegularDeckInfo.HighestNumber => 13; //aces will usually be low.
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList
    {
        get
        {
            if (_level.LevelChosen == 1)
            {
                return new() { EnumSuitList.Spades };
            }
            if (_level.LevelChosen == 2)
            {
                return new() { EnumSuitList.Spades, EnumSuitList.Hearts };
            }
            if (_level.LevelChosen != 3)
            {
                throw new CustomBasicException("Only 3 levels are supposed for the suit list");
            }
            var tempList = EnumSuitList.CompleteList;
            return tempList;
        }
    }
    int IDeckCount.GetDeckCount()
    {
        return 104;
    }
}