namespace Savannah.Core.Data;
[SingletonGame]
public class SavannahDetailClass : IGameInfo, ICardInfo<RegularSimpleCard>,
    IBeginningRegularCards<RegularSimpleCard>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly; //the computer player will skip turn on this one though. (unless i decide to have computer ai).
    string IGameInfo.GameName => "Savannah";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    //this time, attempt to make work on even phones.
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    int ICardInfo<RegularSimpleCard>.CardsToPassOut => 19; //change to what you need.
    BasicList<int> ICardInfo<RegularSimpleCard>.PlayerExcludeList => new();
    BasicList<int> ICardInfo<RegularSimpleCard>.DiscardExcludeList(IListShuffler<RegularSimpleCard> deckList)
    {
        return new(); //most of the time, nothing needs to be excluded.  games like uno or hit the deck will populate.
    }
    bool ICardInfo<RegularSimpleCard>.AddToDiscardAtBeginning => false;
    bool ICardInfo<RegularSimpleCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RegularSimpleCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RegularSimpleCard>.PassOutAll => false;
    bool ICardInfo<RegularSimpleCard>.PlayerGetsCards => true;
    bool ICardInfo<RegularSimpleCard>.NoPass => false;
    bool ICardInfo<RegularSimpleCard>.NeedsDummyHand => false;
    DeckRegularDict<RegularSimpleCard> ICardInfo<RegularSimpleCard>.DummyHand { get; set; } = new DeckRegularDict<RegularSimpleCard>();
    bool ICardInfo<RegularSimpleCard>.HasDrawAnimation => true;
    bool ICardInfo<RegularSimpleCard>.CanSortCardsToBeginWith => false;
    bool IBeginningRegularCards<RegularSimpleCard>.AceLow => true;
    bool IBeginningRegularCards<RegularSimpleCard>.CustomDeck => true;
}