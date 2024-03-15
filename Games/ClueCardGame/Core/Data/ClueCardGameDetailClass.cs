namespace ClueCardGame.Core.Data;
[SingletonGame]
public class ClueCardGameDetailClass : IGameInfo, ICardInfo<ClueCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => true;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Clue Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 4;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
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