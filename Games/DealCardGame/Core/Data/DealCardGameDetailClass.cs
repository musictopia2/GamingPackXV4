namespace DealCardGame.Core.Data;
[SingletonGame]
public class DealCardGameDetailClass : IGameInfo, ICardInfo<DealCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Deal Card Game";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 5;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<DealCardGameCardInformation>.CardsToPassOut => 5; //change to what you need.
    BasicList<int> ICardInfo<DealCardGameCardInformation>.PlayerExcludeList => [];
    BasicList<int> ICardInfo<DealCardGameCardInformation>.DiscardExcludeList(IListShuffler<DealCardGameCardInformation> deckList)
    {
        return [];
    }
    bool ICardInfo<DealCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<DealCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<DealCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<DealCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<DealCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<DealCardGameCardInformation>.NoPass => false;
    bool ICardInfo<DealCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<DealCardGameCardInformation> ICardInfo<DealCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<DealCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<DealCardGameCardInformation>.CanSortCardsToBeginWith => true;
}