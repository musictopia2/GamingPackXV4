namespace Rook.Core.Data;
[SingletonGame]
public class RookDetailClass(RookDelegates delegates) : IGameInfo, ICardInfo<RookCardInformation>, ITrickData
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Rook";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<RookCardInformation>.CardsToPassOut
    {
        get
        {
            if (GlobalClass.Container!.PlayerList!.Count == 4)
            {
                return 9;
            }
            return 12;
        }
    }
    BasicList<int> ICardInfo<RookCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<RookCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<RookCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RookCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RookCardInformation>.PassOutAll => false;
    bool ICardInfo<RookCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<RookCardInformation>.NoPass => false;
    bool ICardInfo<RookCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<RookCardInformation> ICardInfo<RookCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<RookCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<RookCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => delegates.IsDummy!.Invoke();
    BasicList<int> ICardInfo<RookCardInformation>.DiscardExcludeList(IListShuffler<RookCardInformation> deckList)
    {
        return [];
    }
}