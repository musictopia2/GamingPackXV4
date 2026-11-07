namespace YaBlewIt.Core.Data;
[SingletonGame]
public class YaBlewItDetailClass : IGameInfo, ICardInfo<YaBlewItCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType; //can't be networked only.   otherwise, can't test.
    public const string GameName = "Ya Blew It";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 6;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<YaBlewItCardInformation>.CardsToPassOut => 0; //pass out no cards. however, will later pass out a faulty card.
    BasicList<int> ICardInfo<YaBlewItCardInformation>.PlayerExcludeList => [];
    BasicList<int> ICardInfo<YaBlewItCardInformation>.DiscardExcludeList(IListShuffler<YaBlewItCardInformation> deckList)
    {
        return [];
    }
    bool ICardInfo<YaBlewItCardInformation>.AddToDiscardAtBeginning => false; //because the main turn has to draw a card.
    bool ICardInfo<YaBlewItCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<YaBlewItCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<YaBlewItCardInformation>.PassOutAll => false;
    bool ICardInfo<YaBlewItCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<YaBlewItCardInformation>.NoPass => true;
    bool ICardInfo<YaBlewItCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<YaBlewItCardInformation> ICardInfo<YaBlewItCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<YaBlewItCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<YaBlewItCardInformation>.CanSortCardsToBeginWith => true;
}