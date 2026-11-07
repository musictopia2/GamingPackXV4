namespace Savannah.Core.Data;
[SingletonGame]
public class SavannahDetailClass : IGameInfo, ICardInfo<RegularSimpleCard>,
    IBeginningRegularCards<RegularSimpleCard>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType; //the computer player will skip turn on this one though. (unless i decide to have computer ai).
    public const string GameName = "Savannah";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    //this time, attempt to make work on even phones.
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<RegularSimpleCard>.CardsToPassOut => 19; //change to what you need.
    BasicList<int> ICardInfo<RegularSimpleCard>.PlayerExcludeList => [];
    BasicList<int> ICardInfo<RegularSimpleCard>.DiscardExcludeList(IListShuffler<RegularSimpleCard> deckList)
    {
        return []; //most of the time, nothing needs to be excluded.  games like uno or hit the deck will populate.
    }
    bool ICardInfo<RegularSimpleCard>.AddToDiscardAtBeginning => false;
    bool ICardInfo<RegularSimpleCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RegularSimpleCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RegularSimpleCard>.PassOutAll => false;
    bool ICardInfo<RegularSimpleCard>.PlayerGetsCards => true;
    bool ICardInfo<RegularSimpleCard>.NoPass => false;
    bool ICardInfo<RegularSimpleCard>.NeedsDummyHand => false;
    DeckRegularDict<RegularSimpleCard> ICardInfo<RegularSimpleCard>.DummyHand { get; set; } = [];
    bool ICardInfo<RegularSimpleCard>.HasDrawAnimation => true;
    bool ICardInfo<RegularSimpleCard>.CanSortCardsToBeginWith => false;
    bool IBeginningRegularCards<RegularSimpleCard>.AceLow => true;
    bool IBeginningRegularCards<RegularSimpleCard>.CustomDeck => true;
}