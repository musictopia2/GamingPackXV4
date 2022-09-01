namespace Spades2Player.Core.Data;
[SingletonGame]
public class Spades2PlayerDetailClass : IGameInfo, ICardInfo<Spades2PlayerCardInformation>, ITrickData,
    IBeginningRegularCards<Spades2PlayerCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Spades (2 Player)";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<Spades2PlayerCardInformation>.CardsToPassOut => 7;
    BasicList<int> ICardInfo<Spades2PlayerCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<Spades2PlayerCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<Spades2PlayerCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<Spades2PlayerCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<Spades2PlayerCardInformation>.PassOutAll => false;
    bool ICardInfo<Spades2PlayerCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<Spades2PlayerCardInformation>.NoPass => true;
    bool ICardInfo<Spades2PlayerCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<Spades2PlayerCardInformation> ICardInfo<Spades2PlayerCardInformation>.DummyHand { get; set; } = new DeckRegularDict<Spades2PlayerCardInformation>();
    bool ICardInfo<Spades2PlayerCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<Spades2PlayerCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.Spades;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<Spades2PlayerCardInformation>.DiscardExcludeList(IListShuffler<Spades2PlayerCardInformation> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<Spades2PlayerCardInformation>.AceLow => false;
    bool IBeginningRegularCards<Spades2PlayerCardInformation>.CustomDeck => false;
}