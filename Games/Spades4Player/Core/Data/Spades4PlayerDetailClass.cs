namespace Spades4Player.Core.Data;
[SingletonGame]
public class Spades4PlayerDetailClass : IGameInfo, ICardInfo<Spades4PlayerCardInformation>, ITrickData,
    IBeginningRegularCards<Spades4PlayerCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    string IGameInfo.GameName => "Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    int ICardInfo<Spades4PlayerCardInformation>.CardsToPassOut => 7; //change to what you need.
    BasicList<int> ICardInfo<Spades4PlayerCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<Spades4PlayerCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<Spades4PlayerCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<Spades4PlayerCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<Spades4PlayerCardInformation>.PassOutAll => false;
    bool ICardInfo<Spades4PlayerCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<Spades4PlayerCardInformation>.NoPass => false;
    bool ICardInfo<Spades4PlayerCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<Spades4PlayerCardInformation> ICardInfo<Spades4PlayerCardInformation>.DummyHand { get; set; } = new DeckRegularDict<Spades4PlayerCardInformation>();
    BasicList<int> ICardInfo<Spades4PlayerCardInformation>.DiscardExcludeList(IListShuffler<Spades4PlayerCardInformation> deckList)
    {
        return new();
    }
    bool ICardInfo<Spades4PlayerCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<Spades4PlayerCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => false;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    bool IBeginningRegularCards<Spades4PlayerCardInformation>.AceLow => false;
    bool IBeginningRegularCards<Spades4PlayerCardInformation>.CustomDeck => false;
}