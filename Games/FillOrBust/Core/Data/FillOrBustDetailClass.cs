namespace FillOrBust.Core.Data;
[SingletonGame]
public class FillOrBustDetailClass : IGameInfo, ICardInfo<FillOrBustCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Fill Or Bust";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 8;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<FillOrBustCardInformation>.CardsToPassOut => 0;
    BasicList<int> ICardInfo<FillOrBustCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<FillOrBustCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<FillOrBustCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<FillOrBustCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<FillOrBustCardInformation>.PassOutAll => false;
    bool ICardInfo<FillOrBustCardInformation>.PlayerGetsCards => false;
    bool ICardInfo<FillOrBustCardInformation>.NoPass => true;
    bool ICardInfo<FillOrBustCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<FillOrBustCardInformation> ICardInfo<FillOrBustCardInformation>.DummyHand { get; set; } = new DeckRegularDict<FillOrBustCardInformation>();
    bool ICardInfo<FillOrBustCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<FillOrBustCardInformation>.CanSortCardsToBeginWith => false;
    BasicList<int> ICardInfo<FillOrBustCardInformation>.DiscardExcludeList(IListShuffler<FillOrBustCardInformation> deckList)
    {
        return new();
    }
}