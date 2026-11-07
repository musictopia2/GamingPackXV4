namespace DutchBlitz.Core.Data;
[SingletonGame]
public class DutchBlitzDetailClass : IGameInfo, ICardInfo<DutchBlitzCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Dutch Blitz";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => false;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<DutchBlitzCardInformation>.CardsToPassOut => 0;
    BasicList<int> ICardInfo<DutchBlitzCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<DutchBlitzCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<DutchBlitzCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<DutchBlitzCardInformation>.ShowMessageWhenReshuffling => false;
    bool ICardInfo<DutchBlitzCardInformation>.PassOutAll => false;
    bool ICardInfo<DutchBlitzCardInformation>.PlayerGetsCards => false;
    bool ICardInfo<DutchBlitzCardInformation>.NoPass => true;
    bool ICardInfo<DutchBlitzCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<DutchBlitzCardInformation> ICardInfo<DutchBlitzCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<DutchBlitzCardInformation>.HasDrawAnimation => false;
    bool ICardInfo<DutchBlitzCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<DutchBlitzCardInformation>.DiscardExcludeList(IListShuffler<DutchBlitzCardInformation> deckList)
    {
        return [];
    }
}