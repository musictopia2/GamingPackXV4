namespace BasicMultiplayerTrickCardGames.Core.Data;
[SingletonGame]
public class BasicMultiplayerTrickCardGamesDetailClass : IGameInfo, ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>, ITrickData,
    IBeginningRegularCards<BasicMultiplayerTrickCardGamesCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
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
    int ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.CardsToPassOut => 7; //change to what you need.
    BasicList<int> ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.PassOutAll => false;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.NoPass => false;
    bool ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<BasicMultiplayerTrickCardGamesCardInformation> ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.DummyHand { get; set; } = [];
    BasicList<int> ICardInfo<BasicMultiplayerTrickCardGamesCardInformation>.DiscardExcludeList(IListShuffler<BasicMultiplayerTrickCardGamesCardInformation> deckList)
    {
        return [];
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