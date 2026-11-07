namespace FillOrBust.Core.Data;
[SingletonGame]
public class FillOrBustDetailClass : IGameInfo, ICardInfo<FillOrBustCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Fill Or Bust";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 8;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<FillOrBustCardInformation>.CardsToPassOut => 0;
    BasicList<int> ICardInfo<FillOrBustCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<FillOrBustCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<FillOrBustCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<FillOrBustCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<FillOrBustCardInformation>.PassOutAll => false;
    bool ICardInfo<FillOrBustCardInformation>.PlayerGetsCards => false;
    bool ICardInfo<FillOrBustCardInformation>.NoPass => true;
    bool ICardInfo<FillOrBustCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<FillOrBustCardInformation> ICardInfo<FillOrBustCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<FillOrBustCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<FillOrBustCardInformation>.CanSortCardsToBeginWith => false;
    BasicList<int> ICardInfo<FillOrBustCardInformation>.DiscardExcludeList(IListShuffler<FillOrBustCardInformation> deckList)
    {
        return [];
    }
}