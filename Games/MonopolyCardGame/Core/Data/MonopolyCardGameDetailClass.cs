namespace MonopolyCardGame.Core.Data;
[SingletonGame]
public class MonopolyCardGameDetailClass : IGameInfo, ICardInfo<MonopolyCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Monopoly Card Game";
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
    int ICardInfo<MonopolyCardGameCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<MonopolyCardGameCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<MonopolyCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<MonopolyCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<MonopolyCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<MonopolyCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<MonopolyCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<MonopolyCardGameCardInformation>.NoPass => false;
    bool ICardInfo<MonopolyCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<MonopolyCardGameCardInformation> ICardInfo<MonopolyCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<MonopolyCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<MonopolyCardGameCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<MonopolyCardGameCardInformation>.DiscardExcludeList(IListShuffler<MonopolyCardGameCardInformation> deckList)
    {
        return [];
    }
}