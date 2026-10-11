namespace DummyRummy.Core.Data;
[SingletonGame]
public class DummyRummyDetailClass(DummyRummyDelegates delegates) : IGameInfo, ICardInfo<RegularRummyCard>,
    IBeginningRegularCards<RegularRummyCard>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    string IGameInfo.GameName => "Dummy Rummy";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 3;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<RegularRummyCard>.CardsToPassOut => delegates.CardsToPassOut!.Invoke();
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
    bool IBeginningRegularCards<RegularRummyCard>.AceLow => true;
    bool IBeginningRegularCards<RegularRummyCard>.CustomDeck => false;
}