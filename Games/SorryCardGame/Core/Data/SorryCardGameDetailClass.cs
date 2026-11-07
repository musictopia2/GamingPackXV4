namespace SorryCardGame.Core.Data;
[SingletonGame]
public class SorryCardGameDetailClass : IGameInfo, ICardInfo<SorryCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Sorry Card Game";
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
    int ICardInfo<SorryCardGameCardInformation>.CardsToPassOut => 5;
    BasicList<int> ICardInfo<SorryCardGameCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<SorryCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<SorryCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<SorryCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<SorryCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<SorryCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<SorryCardGameCardInformation>.NoPass => false;
    bool ICardInfo<SorryCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<SorryCardGameCardInformation> ICardInfo<SorryCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<SorryCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<SorryCardGameCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<SorryCardGameCardInformation>.DiscardExcludeList(IListShuffler<SorryCardGameCardInformation> deckList)
    {
        return [];
    }
}