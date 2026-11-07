namespace RoundsCardGame.Core.Data;
[SingletonGame]
public class RoundsCardGameDetailClass : IGameInfo, ICardInfo<RoundsCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<RoundsCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Rounds Card Game";
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
    int ICardInfo<RoundsCardGameCardInformation>.CardsToPassOut => 9;
    BasicList<int> ICardInfo<RoundsCardGameCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<RoundsCardGameCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<RoundsCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RoundsCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RoundsCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<RoundsCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<RoundsCardGameCardInformation>.NoPass => false;
    bool ICardInfo<RoundsCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<RoundsCardGameCardInformation> ICardInfo<RoundsCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<RoundsCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<RoundsCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<RoundsCardGameCardInformation>.DiscardExcludeList(IListShuffler<RoundsCardGameCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<RoundsCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<RoundsCardGameCardInformation>.CustomDeck => false;
}