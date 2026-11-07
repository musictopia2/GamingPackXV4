namespace MonasteryCardGame.Core.Data;
[SingletonGame]
public class MonasteryCardGameDetailClass : IGameInfo, ICardInfo<MonasteryCardInfo>,
    IBeginningRegularCards<MonasteryCardInfo>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Monastery Card Game";
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
    int ICardInfo<MonasteryCardInfo>.CardsToPassOut => 9;
    BasicList<int> ICardInfo<MonasteryCardInfo>.PlayerExcludeList => [];
    bool ICardInfo<MonasteryCardInfo>.AddToDiscardAtBeginning => true;
    bool ICardInfo<MonasteryCardInfo>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<MonasteryCardInfo>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<MonasteryCardInfo>.PassOutAll => false;
    bool ICardInfo<MonasteryCardInfo>.PlayerGetsCards => true;
    bool ICardInfo<MonasteryCardInfo>.NoPass => false;
    bool ICardInfo<MonasteryCardInfo>.NeedsDummyHand => false;
    DeckRegularDict<MonasteryCardInfo> ICardInfo<MonasteryCardInfo>.DummyHand { get; set; } = [];
    bool ICardInfo<MonasteryCardInfo>.HasDrawAnimation => true;
    bool ICardInfo<MonasteryCardInfo>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<MonasteryCardInfo>.DiscardExcludeList(IListShuffler<MonasteryCardInfo> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<MonasteryCardInfo>.AceLow => true;
    bool IBeginningRegularCards<MonasteryCardInfo>.CustomDeck => true;
}