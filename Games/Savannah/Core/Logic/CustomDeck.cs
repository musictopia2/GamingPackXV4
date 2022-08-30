namespace Savannah.Core.Logic;
public class CustomDeck : IRegularDeckInfo
{
    private readonly SavannahGameContainer _gameContainer;
    public CustomDeck(SavannahGameContainer gameContainer)
    {
        _gameContainer = gameContainer;
    }
    int IRegularDeckInfo.HowManyDecks => _gameContainer.PlayerList!.Count;
    bool IRegularDeckInfo.UseJokers => true;
    int IRegularDeckInfo.GetExtraJokers => _gameContainer.PlayerList!.Count * 2;
    int IRegularDeckInfo.LowestNumber => 1;
    int IRegularDeckInfo.HighestNumber => 13;
    BasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new();
    BasicList<EnumSuitList> IRegularDeckInfo.SuitList => EnumSuitList.CompleteList;
    //eventually will change to allow jokers.
    int IDeckCount.GetDeckCount()
    {
        return _gameContainer.DeckCount;
    }
}