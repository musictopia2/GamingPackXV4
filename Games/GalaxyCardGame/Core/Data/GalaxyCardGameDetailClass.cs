namespace GalaxyCardGame.Core.Data;
[SingletonGame]
public class GalaxyCardGameDetailClass(GalaxyDelegates delegates) : IGameInfo, ICardInfo<GalaxyCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<GalaxyCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Galaxy Card Game";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 2;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<GalaxyCardGameCardInformation>.CardsToPassOut => 9;
    BasicList<int> ICardInfo<GalaxyCardGameCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<GalaxyCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<GalaxyCardGameCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<GalaxyCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<GalaxyCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<GalaxyCardGameCardInformation>.PlayerGetsCards => delegates.PlayerGetCards!.Invoke();
    bool ICardInfo<GalaxyCardGameCardInformation>.NoPass => false;
    bool ICardInfo<GalaxyCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<GalaxyCardGameCardInformation> ICardInfo<GalaxyCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<GalaxyCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<GalaxyCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => false;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<GalaxyCardGameCardInformation>.DiscardExcludeList(IListShuffler<GalaxyCardGameCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<GalaxyCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<GalaxyCardGameCardInformation>.CustomDeck => false;
}