namespace BasicMultiplayerMiscCardGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerMiscCardGamesDetailClass : IGameInfo, ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>
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
    int ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.CardsToPassOut => 7; //change to what you need.
    BasicList<int> ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.PlayerExcludeList => new();
    BasicList<int> ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.DiscardExcludeList(IListShuffler<BasicMultiplayerMiscCardGamesCardInformation> deckList)
    {
        return new();
    }
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.PassOutAll => false;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.NoPass => false;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<BasicMultiplayerMiscCardGamesCardInformation> ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.DummyHand { get; set; } = new DeckRegularDict<BasicMultiplayerMiscCardGamesCardInformation>();
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.CanSortCardsToBeginWith => true;
}