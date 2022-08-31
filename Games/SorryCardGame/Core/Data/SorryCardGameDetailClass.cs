namespace SorryCardGame.Core.Data;
[SingletonGame]
public class SorryCardGameDetailClass : IGameInfo, ICardInfo<SorryCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Sorry Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<SorryCardGameCardInformation>.CardsToPassOut => 5;
    BasicList<int> ICardInfo<SorryCardGameCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<SorryCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<SorryCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<SorryCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<SorryCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<SorryCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<SorryCardGameCardInformation>.NoPass => false;
    bool ICardInfo<SorryCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<SorryCardGameCardInformation> ICardInfo<SorryCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<SorryCardGameCardInformation>();
    bool ICardInfo<SorryCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<SorryCardGameCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<SorryCardGameCardInformation>.DiscardExcludeList(IListShuffler<SorryCardGameCardInformation> deckList)
    {
        return new();
    }
}