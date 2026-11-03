namespace MonasteryCardGame.Core.Data;
[SingletonGame]
public class MonasteryCardGameDetailClass : IGameInfo, ICardInfo<MonasteryCardInfo>,
    IBeginningRegularCards<MonasteryCardInfo>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    public const string GameName = "Monastery Card Game";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
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