namespace SnagCardGame.Core.Data;
[SingletonGame]
public class SnagCardGameDetailClass : IGameInfo, ICardInfo<SnagCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<SnagCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Snag Card Game";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 2;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<SnagCardGameCardInformation>.CardsToPassOut => 5;
    BasicList<int> ICardInfo<SnagCardGameCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<SnagCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<SnagCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<SnagCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<SnagCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<SnagCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<SnagCardGameCardInformation>.NoPass => false;
    bool ICardInfo<SnagCardGameCardInformation>.NeedsDummyHand => true;
    DeckRegularDict<SnagCardGameCardInformation> ICardInfo<SnagCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<SnagCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<SnagCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => true;
    BasicList<int> ICardInfo<SnagCardGameCardInformation>.DiscardExcludeList(IListShuffler<SnagCardGameCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<SnagCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<SnagCardGameCardInformation>.CustomDeck => false;
}