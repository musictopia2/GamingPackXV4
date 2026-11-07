namespace SkuckCardGame.Core.Data;
[SingletonGame]
public class SkuckCardGameDetailClass : IGameInfo, ICardInfo<SkuckCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<SkuckCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Skuck Card Game";
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
    int ICardInfo<SkuckCardGameCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<SkuckCardGameCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<SkuckCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<SkuckCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<SkuckCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<SkuckCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<SkuckCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<SkuckCardGameCardInformation>.NoPass => false;
    bool ICardInfo<SkuckCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<SkuckCardGameCardInformation> ICardInfo<SkuckCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<SkuckCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<SkuckCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => true;
    BasicList<int> ICardInfo<SkuckCardGameCardInformation>.DiscardExcludeList(IListShuffler<SkuckCardGameCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<SkuckCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<SkuckCardGameCardInformation>.CustomDeck => false;
}