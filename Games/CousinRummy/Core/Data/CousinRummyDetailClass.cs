namespace CousinRummy.Core.Data;
[SingletonGame]
public class CousinRummyDetailClass : IGameInfo, ICardInfo<RegularRummyCard>,
    IBeginningRegularCards<RegularRummyCard>, IRegularDeckWild
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Cousin Rummy";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<RegularRummyCard>.CardsToPassOut => 12;
    BasicList<int> ICardInfo<RegularRummyCard>.PlayerExcludeList => new();
    bool ICardInfo<RegularRummyCard>.AddToDiscardAtBeginning => true;
    bool ICardInfo<RegularRummyCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RegularRummyCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RegularRummyCard>.PassOutAll => false;
    bool ICardInfo<RegularRummyCard>.PlayerGetsCards => true;
    bool ICardInfo<RegularRummyCard>.NoPass => false;
    bool ICardInfo<RegularRummyCard>.NeedsDummyHand => false;
    DeckRegularDict<RegularRummyCard> ICardInfo<RegularRummyCard>.DummyHand { get; set; } = new DeckRegularDict<RegularRummyCard>();
    bool ICardInfo<RegularRummyCard>.HasDrawAnimation => true;
    bool ICardInfo<RegularRummyCard>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<RegularRummyCard>.DiscardExcludeList(IListShuffler<RegularRummyCard> deckList)
    {
        return deckList.Where(x => x.IsObjectWild).Select(x => x.Deck).ToBasicList();
    }
    bool IRegularDeckWild.IsWild(IRegularCard thisCard)
    {
        return thisCard.Value == EnumRegularCardValueList.Two || thisCard.Value == EnumRegularCardValueList.Joker;
    }
    bool IBeginningRegularCards<RegularRummyCard>.AceLow => false;
    bool IBeginningRegularCards<RegularRummyCard>.CustomDeck => true;
}