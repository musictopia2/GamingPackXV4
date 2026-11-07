namespace Racko.Core.Data;
[SingletonGame]
public class RackoDetailClass : IGameInfo, ICardInfo<RackoCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers; //can no longer have extra computer players because there was some bugs.  decided for now, not worth fixing.  not as important.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Racko";
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
    int ICardInfo<RackoCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<RackoCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<RackoCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<RackoCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RackoCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RackoCardInformation>.PassOutAll => false;
    bool ICardInfo<RackoCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<RackoCardInformation>.NoPass => false;
    bool ICardInfo<RackoCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<RackoCardInformation> ICardInfo<RackoCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<RackoCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<RackoCardInformation>.CanSortCardsToBeginWith => false;
    BasicList<int> ICardInfo<RackoCardInformation>.DiscardExcludeList(IListShuffler<RackoCardInformation> deckList)
    {
        return [];
    }
}