namespace A8RoundRummy.Core.Data;
[SingletonGame]
public class A8RoundRummyDetailClass : IGameInfo, ICardInfo<A8RoundRummyCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "8 Round Rummy";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 6;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<A8RoundRummyCardInformation>.CardsToPassOut => 7;
    BasicList<int> ICardInfo<A8RoundRummyCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<A8RoundRummyCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<A8RoundRummyCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<A8RoundRummyCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<A8RoundRummyCardInformation>.PassOutAll => false;
    bool ICardInfo<A8RoundRummyCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<A8RoundRummyCardInformation>.NoPass => false;
    bool ICardInfo<A8RoundRummyCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<A8RoundRummyCardInformation> ICardInfo<A8RoundRummyCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<A8RoundRummyCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<A8RoundRummyCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<A8RoundRummyCardInformation>.DiscardExcludeList(IListShuffler<A8RoundRummyCardInformation> deckList)
    {
        return deckList.Where(x => x.CardType == EnumCardType.Reverse).Select(x => x.Deck).ToBasicList();
    }
}