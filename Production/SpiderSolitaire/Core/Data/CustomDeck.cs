namespace SpiderSolitaire.Core.Data;
public class CustomDeck(LevelClass level) : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks
    {
        get
        {
            if (level.LevelChosen == 3)
            {
                return 2;
            }
            if (level.LevelChosen == 2)
            {
                return 4;
            }
            if (level.LevelChosen == 1)
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
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => [];
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList
    {
        get
        {
            if (level.LevelChosen == 1)
            {
                return [EnumSuitList.Spades];
            }
            if (level.LevelChosen == 2)
            {
                return [EnumSuitList.Spades, EnumSuitList.Hearts];
            }
            if (level.LevelChosen != 3)
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