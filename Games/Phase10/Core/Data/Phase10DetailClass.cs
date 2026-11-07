namespace Phase10.Core.Data;
[SingletonGame]
public class Phase10DetailClass : IGameInfo, ICardInfo<Phase10CardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Phase 10";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 7;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<Phase10CardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<Phase10CardInformation>.PlayerExcludeList => [];
    bool ICardInfo<Phase10CardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<Phase10CardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<Phase10CardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<Phase10CardInformation>.PassOutAll => false;
    bool ICardInfo<Phase10CardInformation>.PlayerGetsCards => true;
    bool ICardInfo<Phase10CardInformation>.NoPass => false;
    bool ICardInfo<Phase10CardInformation>.NeedsDummyHand => false;
    DeckRegularDict<Phase10CardInformation> ICardInfo<Phase10CardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<Phase10CardInformation>.HasDrawAnimation => true;
    bool ICardInfo<Phase10CardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<Phase10CardInformation>.DiscardExcludeList(IListShuffler<Phase10CardInformation> deckList)
    {
        return [];
    }
}