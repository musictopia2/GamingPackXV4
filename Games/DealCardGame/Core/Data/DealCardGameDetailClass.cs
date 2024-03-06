namespace DealCardGame.Core.Data;
[SingletonGame]
public class DealCardGameDetailClass : IGameInfo, ICardInfo<DealCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    string IGameInfo.GameName => "Deal Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 5;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
    int ICardInfo<DealCardGameCardInformation>.CardsToPassOut => 5; //change to what you need.
    BasicList<int> ICardInfo<DealCardGameCardInformation>.PlayerExcludeList => [];
    BasicList<int> ICardInfo<DealCardGameCardInformation>.DiscardExcludeList(IListShuffler<DealCardGameCardInformation> deckList)
    {
        return [];
    }
    bool ICardInfo<DealCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<DealCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<DealCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<DealCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<DealCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<DealCardGameCardInformation>.NoPass => false;
    bool ICardInfo<DealCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<DealCardGameCardInformation> ICardInfo<DealCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<DealCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<DealCardGameCardInformation>.CanSortCardsToBeginWith => true;
}