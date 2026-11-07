namespace FourSuitRummy.Core.Data;
[SingletonGame]
public class FourSuitRummyDetailClass : IGameInfo, ICardInfo<RegularRummyCard>,
    IBeginningRegularCards<RegularRummyCard>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Four Suit Rummy";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 2;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<RegularRummyCard>.CardsToPassOut => 12;
    BasicList<int> ICardInfo<RegularRummyCard>.PlayerExcludeList => [];
    bool ICardInfo<RegularRummyCard>.AddToDiscardAtBeginning => true;
    bool ICardInfo<RegularRummyCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RegularRummyCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RegularRummyCard>.PassOutAll => false;
    bool ICardInfo<RegularRummyCard>.PlayerGetsCards => true;
    bool ICardInfo<RegularRummyCard>.NoPass => false;
    bool ICardInfo<RegularRummyCard>.NeedsDummyHand => false;
    DeckRegularDict<RegularRummyCard> ICardInfo<RegularRummyCard>.DummyHand { get; set; } = [];
    bool ICardInfo<RegularRummyCard>.HasDrawAnimation => true;
    bool ICardInfo<RegularRummyCard>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<RegularRummyCard>.DiscardExcludeList(IListShuffler<RegularRummyCard> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<RegularRummyCard>.AceLow => false;
    bool IBeginningRegularCards<RegularRummyCard>.CustomDeck => false;
}