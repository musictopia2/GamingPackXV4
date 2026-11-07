namespace MilkRun.Core.Data;
[SingletonGame]
public class MilkRunDetailClass : IGameInfo, ICardInfo<MilkRunCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Milk Run";
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
    int ICardInfo<MilkRunCardInformation>.CardsToPassOut => 6;
    BasicList<int> ICardInfo<MilkRunCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<MilkRunCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<MilkRunCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<MilkRunCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<MilkRunCardInformation>.PassOutAll => false;
    bool ICardInfo<MilkRunCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<MilkRunCardInformation>.NoPass => false;
    bool ICardInfo<MilkRunCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<MilkRunCardInformation> ICardInfo<MilkRunCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<MilkRunCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<MilkRunCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<MilkRunCardInformation>.DiscardExcludeList(IListShuffler<MilkRunCardInformation> deckList)
    {
        return [];
    }
}