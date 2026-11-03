namespace HitTheDeck.Core.Data;
[SingletonGame]
public class HitTheDeckDetailClass : IGameInfo, ICardInfo<HitTheDeckCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false; //too many bugs with extra computer players.  this means for multiplayer games can't have extra computer players.  hint.  with draw 4 cards.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "Hit The Deck";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<HitTheDeckCardInformation>.CardsToPassOut => 7;
    BasicList<int> ICardInfo<HitTheDeckCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<HitTheDeckCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<HitTheDeckCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<HitTheDeckCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<HitTheDeckCardInformation>.PassOutAll => false;
    bool ICardInfo<HitTheDeckCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<HitTheDeckCardInformation>.NoPass => false;
    bool ICardInfo<HitTheDeckCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<HitTheDeckCardInformation> ICardInfo<HitTheDeckCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<HitTheDeckCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<HitTheDeckCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<HitTheDeckCardInformation>.DiscardExcludeList(IListShuffler<HitTheDeckCardInformation> deckList)
    {
        return deckList.Where(x => x.CardType == EnumTypeList.Flip).Select(x => x.Deck).ToBasicList(); //the first card on the discard cannot be flip now.
    }
}