namespace DutchBlitz.Core.Data;
[SingletonGame]
public class DutchBlitzDetailClass : IGameInfo, ICardInfo<DutchBlitzCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Dutch Blitz";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => false;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait;
    int ICardInfo<DutchBlitzCardInformation>.CardsToPassOut => 0;
    BasicList<int> ICardInfo<DutchBlitzCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<DutchBlitzCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<DutchBlitzCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<DutchBlitzCardInformation>.ShowMessageWhenReshuffling => false;
    bool ICardInfo<DutchBlitzCardInformation>.PassOutAll => false;
    bool ICardInfo<DutchBlitzCardInformation>.PlayerGetsCards => false;
    bool ICardInfo<DutchBlitzCardInformation>.NoPass => true;
    bool ICardInfo<DutchBlitzCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<DutchBlitzCardInformation> ICardInfo<DutchBlitzCardInformation>.DummyHand { get; set; } = new DeckRegularDict<DutchBlitzCardInformation>();
    bool ICardInfo<DutchBlitzCardInformation>.HasDrawAnimation => false;
    bool ICardInfo<DutchBlitzCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<DutchBlitzCardInformation>.DiscardExcludeList(IListShuffler<DutchBlitzCardInformation> deckList)
    {
        return new();
    }
}