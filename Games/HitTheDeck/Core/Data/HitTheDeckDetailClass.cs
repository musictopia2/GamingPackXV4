namespace HitTheDeck.Core.Data;
[SingletonGame]
public class HitTheDeckDetailClass : IGameInfo, ICardInfo<HitTheDeckCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false; //too many bugs with extra computer players.  this means for multiplayer games can't have extra computer players.  hint.  with draw 4 cards.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Hit The Deck";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<HitTheDeckCardInformation>.CardsToPassOut => 7;
    BasicList<int> ICardInfo<HitTheDeckCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<HitTheDeckCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<HitTheDeckCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<HitTheDeckCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<HitTheDeckCardInformation>.PassOutAll => false;
    bool ICardInfo<HitTheDeckCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<HitTheDeckCardInformation>.NoPass => false;
    bool ICardInfo<HitTheDeckCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<HitTheDeckCardInformation> ICardInfo<HitTheDeckCardInformation>.DummyHand { get; set; } = new DeckRegularDict<HitTheDeckCardInformation>();
    bool ICardInfo<HitTheDeckCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<HitTheDeckCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<HitTheDeckCardInformation>.DiscardExcludeList(IListShuffler<HitTheDeckCardInformation> deckList)
    {
        return deckList.Where(x => x.CardType == EnumTypeList.Flip).Select(x => x.Deck).ToBasicList(); //the first card on the discard cannot be flip now.
    }
}