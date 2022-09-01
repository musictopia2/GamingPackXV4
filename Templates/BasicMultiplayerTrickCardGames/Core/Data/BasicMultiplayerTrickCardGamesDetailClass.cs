namespace BasicMultiplayerTrickCardGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerTrickCardGamesDetailClass : IGameInfo, ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>, ITrickData,
    IBeginningRegularCards<BasicMultiplayerTrickCardGamesCardInformation>
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
    int ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.CardsToPassOut => 7; //change to what you need.
    BasicList<int> ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.PassOutAll => false;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.NoPass => false;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<BasicMultiplayerTrickCardGamesCardInformation> ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.DummyHand { get; set; } = new DeckRegularDict<BasicMultiplayerTrickCardGamesCardInformation>();
    BasicList<int> ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.DiscardExcludeList(IListShuffler<BasicMultiplayerTrickCardGamesCardInformation> deckList)
    {
        return new();
    }
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => false;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    bool IBeginningRegularCards<BasicMultiplayerTrickCardGamesCardInformation>.AceLow => false;
    bool IBeginningRegularCards<BasicMultiplayerTrickCardGamesCardInformation>.CustomDeck => false;
}