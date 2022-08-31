namespace Uno.Core.Data;
[SingletonGame]
public class UnoDetailClass : IGameInfo, ICardInfo<UnoCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => true;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Uno";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<UnoCardInformation>.CardsToPassOut => 7;
    BasicList<int> ICardInfo<UnoCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<UnoCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<UnoCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<UnoCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<UnoCardInformation>.PassOutAll => false;
    bool ICardInfo<UnoCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<UnoCardInformation>.NoPass => false;
    bool ICardInfo<UnoCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<UnoCardInformation> ICardInfo<UnoCardInformation>.DummyHand { get; set; } = new DeckRegularDict<UnoCardInformation>();
    bool ICardInfo<UnoCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<UnoCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<UnoCardInformation>.DiscardExcludeList(IListShuffler<UnoCardInformation> deckList)
    {
        return deckList.Where(x => x.WhichType == EnumCardTypeList.Wild && x.Draw == 4).Select(x => x.Deck).ToBasicList();
    }
}