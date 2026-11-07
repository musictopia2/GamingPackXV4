namespace BasicMultiplayerMiscCardGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerMiscCardGamesDetailClass : IGameInfo, ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Game";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.CardsToPassOut => 7; //change to what you need.
    BasicList<int> ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.PlayerExcludeList => [];
    BasicList<int> ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.DiscardExcludeList(IListShuffler<BasicMultiplayerMiscCardGamesCardInformation> deckList)
    {
        return [];
    }
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.PassOutAll => false;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.NoPass => false;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<BasicMultiplayerMiscCardGamesCardInformation> ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<BasicMultiplayerMiscCardGamesCardInformation>.CanSortCardsToBeginWith => true;
}