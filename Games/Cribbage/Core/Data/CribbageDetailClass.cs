namespace Cribbage.Core.Data;
[SingletonGame]
public class CribbageDetailClass(CribbageDelegates delegates) : IGameInfo, ICardInfo<CribbageCard>,
    IBeginningRegularCards<CribbageCard>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Cribbage";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 3;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<CribbageCard>.CardsToPassOut
    {
        get
        {
            if (delegates.GetPlayerCount == null)
            {
                throw new CustomBasicException("Nobody is handling get player count.  Rethink");
            }
            int count = delegates.GetPlayerCount.Invoke();
            if (count == 2)
            {
                return 6;
            }
            else if (count == 3)
            {
                return 5;
            }
            else
            {
                throw new CustomBasicException("Only 2 or 3 players are supported");
            }
        }
    }
    BasicList<int> ICardInfo<CribbageCard>.PlayerExcludeList => [];
    bool ICardInfo<CribbageCard>.AddToDiscardAtBeginning => true;
    bool ICardInfo<CribbageCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<CribbageCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<CribbageCard>.PassOutAll => false;
    bool ICardInfo<CribbageCard>.PlayerGetsCards => true;
    bool ICardInfo<CribbageCard>.NoPass => false;
    bool ICardInfo<CribbageCard>.NeedsDummyHand => false;
    DeckRegularDict<CribbageCard> ICardInfo<CribbageCard>.DummyHand { get; set; } = [];
    bool ICardInfo<CribbageCard>.HasDrawAnimation => true;
    bool ICardInfo<CribbageCard>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<CribbageCard>.DiscardExcludeList(IListShuffler<CribbageCard> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<CribbageCard>.AceLow => true;
    bool IBeginningRegularCards<CribbageCard>.CustomDeck => false;
}