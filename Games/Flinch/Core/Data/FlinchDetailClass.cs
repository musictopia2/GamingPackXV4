namespace Flinch.Core.Data;
[SingletonGame]
public class FlinchDetailClass : IGameInfo, ICardInfo<FlinchCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => true;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Flinch";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<FlinchCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<FlinchCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<FlinchCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<FlinchCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<FlinchCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<FlinchCardInformation>.PassOutAll => false;
    bool ICardInfo<FlinchCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<FlinchCardInformation>.NoPass => false;
    bool ICardInfo<FlinchCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<FlinchCardInformation> ICardInfo<FlinchCardInformation>.DummyHand { get; set; } = new DeckRegularDict<FlinchCardInformation>();
    bool ICardInfo<FlinchCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<FlinchCardInformation>.CanSortCardsToBeginWith => false;
    BasicList<int> ICardInfo<FlinchCardInformation>.DiscardExcludeList(IListShuffler<FlinchCardInformation> deckList)
    {
        return new();
    }
}