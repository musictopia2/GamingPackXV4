namespace ClueCardGame.Core.Data;
[SingletonGame]
public class ClueCardGameDetailClass : IGameInfo, ICardInfo<ClueCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = true;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Clue Card Game";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 4;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<ClueCardGameCardInformation>.CardsToPassOut => 3; //change to what you need.
    BasicList<int> ICardInfo<ClueCardGameCardInformation>.PlayerExcludeList => ClueCardGameMainGameClass.ExcludeList;
    BasicList<int> ICardInfo<ClueCardGameCardInformation>.DiscardExcludeList(IListShuffler<ClueCardGameCardInformation> deckList)
    {
        return [];
    }
    bool ICardInfo<ClueCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<ClueCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<ClueCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<ClueCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<ClueCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<ClueCardGameCardInformation>.NoPass => false;
    bool ICardInfo<ClueCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<ClueCardGameCardInformation> ICardInfo<ClueCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<ClueCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<ClueCardGameCardInformation>.CanSortCardsToBeginWith => true;
}