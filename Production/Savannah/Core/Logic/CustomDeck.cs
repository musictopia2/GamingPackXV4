namespace Savannah.Core.Logic;
public class CustomDeck(SavannahGameContainer gameContainer) : IRegularDeckInfo
{
    int IRegularDeckInfo.HowManyDecks => gameContainer.PlayerList!.Count;
    bool IRegularDeckInfo.UseJokers => true;
    int IRegularDeckInfo.GetExtraJokers => gameContainer.PlayerList!.Count * 2;
    int IRegularDeckInfo.LowestNumber => 1;
    int IRegularDeckInfo.HighestNumber => 13;
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => [];
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;
    //eventually will change to allow jokers.
    int IDeckCount.GetDeckCount()
    {
        return gameContainer.DeckCount;
    }
}