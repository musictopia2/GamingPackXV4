namespace YaBlewIt.Core.Data;
[SingletonGame]
public class YaBlewItDetailClass : IGameInfo, ICardInfo<YaBlewItCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //can't be networked only.   otherwise, can't test.
    string IGameInfo.GameName => "Ya Blew It";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.LargeDevices; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    int ICardInfo<YaBlewItCardInformation>.CardsToPassOut => 0; //pass out no cards. however, will later pass out a faulty card.
    BasicList<int> ICardInfo<YaBlewItCardInformation>.PlayerExcludeList => new();
    BasicList<int> ICardInfo<YaBlewItCardInformation>.DiscardExcludeList(IListShuffler<YaBlewItCardInformation> deckList)
    {
        return new();
    }
    bool ICardInfo<YaBlewItCardInformation>.AddToDiscardAtBeginning => false; //because the main turn has to draw a card.
    bool ICardInfo<YaBlewItCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<YaBlewItCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<YaBlewItCardInformation>.PassOutAll => false;
    bool ICardInfo<YaBlewItCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<YaBlewItCardInformation>.NoPass => true;
    bool ICardInfo<YaBlewItCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<YaBlewItCardInformation> ICardInfo<YaBlewItCardInformation>.DummyHand { get; set; } = new DeckRegularDict<YaBlewItCardInformation>();
    bool ICardInfo<YaBlewItCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<YaBlewItCardInformation>.CanSortCardsToBeginWith => true;
}