namespace A8RoundRummy.Core.Data;
[SingletonGame]
public class A8RoundRummyDetailClass : IGameInfo, ICardInfo<A8RoundRummyCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    string IGameInfo.GameName => "8 Round Rummy";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<A8RoundRummyCardInformation>.CardsToPassOut => 7;
    BasicList<int> ICardInfo<A8RoundRummyCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<A8RoundRummyCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<A8RoundRummyCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<A8RoundRummyCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<A8RoundRummyCardInformation>.PassOutAll => false;
    bool ICardInfo<A8RoundRummyCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<A8RoundRummyCardInformation>.NoPass => false;
    bool ICardInfo<A8RoundRummyCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<A8RoundRummyCardInformation> ICardInfo<A8RoundRummyCardInformation>.DummyHand { get; set; } = new DeckRegularDict<A8RoundRummyCardInformation>();
    bool ICardInfo<A8RoundRummyCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<A8RoundRummyCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<A8RoundRummyCardInformation>.DiscardExcludeList(IListShuffler<A8RoundRummyCardInformation> deckList)
    {
        return deckList.Where(x => x.CardType == EnumCardType.Reverse).Select(x => x.Deck).ToBasicList();
    }
}