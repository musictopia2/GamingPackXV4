namespace Chinazo.Core.Data;
[SingletonGame]
public class ChinazoDetailClass(ChinazoDelegates delegates) : IGameInfo, ICardInfo<ChinazoCard>,
    IBeginningRegularCards<ChinazoCard>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Chinazo";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<ChinazoCard>.CardsToPassOut => delegates.CardsToPassOut!.Invoke();
    BasicList<int> ICardInfo<ChinazoCard>.PlayerExcludeList => [];
    bool ICardInfo<ChinazoCard>.AddToDiscardAtBeginning => true;
    bool ICardInfo<ChinazoCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<ChinazoCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<ChinazoCard>.PassOutAll => false;
    bool ICardInfo<ChinazoCard>.PlayerGetsCards => true;
    bool ICardInfo<ChinazoCard>.NoPass => false;
    bool ICardInfo<ChinazoCard>.NeedsDummyHand => false;
    DeckRegularDict<ChinazoCard> ICardInfo<ChinazoCard>.DummyHand { get; set; } = [];
    bool ICardInfo<ChinazoCard>.HasDrawAnimation => true;
    bool ICardInfo<ChinazoCard>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<ChinazoCard>.DiscardExcludeList(IListShuffler<ChinazoCard> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<ChinazoCard>.AceLow => false;
    bool IBeginningRegularCards<ChinazoCard>.CustomDeck => true;
}