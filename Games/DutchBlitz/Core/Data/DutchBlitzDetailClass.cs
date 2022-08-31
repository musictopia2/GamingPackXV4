namespace DutchBlitz.Core.Data;
[SingletonGame]
public class DutchBlitzDetailClass : IGameInfo, ICardInfo<DutchBlitzCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    string IGameInfo.GameName => "Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    int ICardInfo<DutchBlitzCardInformation>.CardsToPassOut => 7; //change to what you need.
    BasicList<int> ICardInfo<DutchBlitzCardInformation>.PlayerExcludeList => new();
    BasicList<int> ICardInfo<DutchBlitzCardInformation>.DiscardExcludeList(IListShuffler<DutchBlitzCardInformation> deckList)
    {
        return new();
    }
    bool ICardInfo<DutchBlitzCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<DutchBlitzCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<DutchBlitzCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<DutchBlitzCardInformation>.PassOutAll => false;
    bool ICardInfo<DutchBlitzCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<DutchBlitzCardInformation>.NoPass => false;
    bool ICardInfo<DutchBlitzCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<DutchBlitzCardInformation> ICardInfo<DutchBlitzCardInformation>.DummyHand { get; set; } = new DeckRegularDict<DutchBlitzCardInformation>();
    bool ICardInfo<DutchBlitzCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<DutchBlitzCardInformation>.CanSortCardsToBeginWith => true;
}