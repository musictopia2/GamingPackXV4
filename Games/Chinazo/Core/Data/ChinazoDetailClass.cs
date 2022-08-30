namespace Chinazo.Core.Data;
[SingletonGame]
public class ChinazoDetailClass : IGameInfo, ICardInfo<ChinazoCard>,
    IBeginningRegularCards<ChinazoCard>
{
    private readonly ChinazoDelegates _delegates;
    public ChinazoDetailClass(ChinazoDelegates delegates)
    {
        _delegates = delegates;
    }
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Chinazo";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<ChinazoCard>.CardsToPassOut => _delegates.CardsToPassOut!.Invoke();
    BasicList<int> ICardInfo<ChinazoCard>.PlayerExcludeList => new();
    bool ICardInfo<ChinazoCard>.AddToDiscardAtBeginning => true;
    bool ICardInfo<ChinazoCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<ChinazoCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<ChinazoCard>.PassOutAll => false;
    bool ICardInfo<ChinazoCard>.PlayerGetsCards => true;
    bool ICardInfo<ChinazoCard>.NoPass => false;
    bool ICardInfo<ChinazoCard>.NeedsDummyHand => false;
    DeckRegularDict<ChinazoCard> ICardInfo<ChinazoCard>.DummyHand { get; set; } = new DeckRegularDict<ChinazoCard>();
    bool ICardInfo<ChinazoCard>.HasDrawAnimation => true;
    bool ICardInfo<ChinazoCard>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<ChinazoCard>.DiscardExcludeList(IListShuffler<ChinazoCard> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<ChinazoCard>.AceLow => false;
    bool IBeginningRegularCards<ChinazoCard>.CustomDeck => true;
}