namespace MilkRun.Core.Data;
[SingletonGame]
public class MilkRunDetailClass : IGameInfo, ICardInfo<MilkRunCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Milk Run";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<MilkRunCardInformation>.CardsToPassOut => 6;
    BasicList<int> ICardInfo<MilkRunCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<MilkRunCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<MilkRunCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<MilkRunCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<MilkRunCardInformation>.PassOutAll => false;
    bool ICardInfo<MilkRunCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<MilkRunCardInformation>.NoPass => false;
    bool ICardInfo<MilkRunCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<MilkRunCardInformation> ICardInfo<MilkRunCardInformation>.DummyHand { get; set; } = new DeckRegularDict<MilkRunCardInformation>();
    bool ICardInfo<MilkRunCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<MilkRunCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<MilkRunCardInformation>.DiscardExcludeList(IListShuffler<MilkRunCardInformation> deckList)
    {
        return new();
    }
}