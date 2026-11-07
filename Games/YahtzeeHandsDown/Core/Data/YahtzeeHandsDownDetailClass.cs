namespace YahtzeeHandsDown.Core.Data;
[SingletonGame]
public class YahtzeeHandsDownDetailClass : IGameInfo, ICardInfo<YahtzeeHandsDownCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Yahtzee Hands Down";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 6;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<YahtzeeHandsDownCardInformation>.CardsToPassOut => 5;
    BasicList<int> ICardInfo<YahtzeeHandsDownCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<YahtzeeHandsDownCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<YahtzeeHandsDownCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<YahtzeeHandsDownCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<YahtzeeHandsDownCardInformation>.PassOutAll => false;
    bool ICardInfo<YahtzeeHandsDownCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<YahtzeeHandsDownCardInformation>.NoPass => false;
    bool ICardInfo<YahtzeeHandsDownCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<YahtzeeHandsDownCardInformation> ICardInfo<YahtzeeHandsDownCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<YahtzeeHandsDownCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<YahtzeeHandsDownCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<YahtzeeHandsDownCardInformation>.DiscardExcludeList(IListShuffler<YahtzeeHandsDownCardInformation> deckList)
    {
        return [];
    }
}