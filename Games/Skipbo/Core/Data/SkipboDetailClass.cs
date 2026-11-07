namespace Skipbo.Core.Data;
[SingletonGame]
public class SkipboDetailClass(SkipboDelegates delegates) : IGameInfo, ICardInfo<SkipboCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = true;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "SkipBo";
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
    int ICardInfo<SkipboCardInformation>.CardsToPassOut
    {
        get
        {
            if (delegates.GetPlayerCount == null)
            {
                throw new CustomBasicException("Nobody is getting player count.  Rethink");
            }
            int count = delegates.GetPlayerCount.Invoke();
            if (count == 2)
            {
                return 30;
            }
            return 20;
        }
    }
    BasicList<int> ICardInfo<SkipboCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<SkipboCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<SkipboCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<SkipboCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<SkipboCardInformation>.PassOutAll => false;
    bool ICardInfo<SkipboCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<SkipboCardInformation>.NoPass => false;
    bool ICardInfo<SkipboCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<SkipboCardInformation> ICardInfo<SkipboCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<SkipboCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<SkipboCardInformation>.CanSortCardsToBeginWith => false;
    BasicList<int> ICardInfo<SkipboCardInformation>.DiscardExcludeList(IListShuffler<SkipboCardInformation> deckList)
    {
        return [];
    }
}