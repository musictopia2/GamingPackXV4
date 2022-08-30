namespace MonasteryCardGame.Core.Logic;
public class CustomDeck : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => 2;

    bool IRegularDeckInfo.UseJokers => false;

    int IRegularDeckInfo.GetExtraJokers => 0;

    int IRegularDeckInfo.LowestNumber => 1;

    int IRegularDeckInfo.HighestNumber => 13;

    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();

    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;

    int IDeckCount.GetDeckCount()
    {
        return 104;
    }
}