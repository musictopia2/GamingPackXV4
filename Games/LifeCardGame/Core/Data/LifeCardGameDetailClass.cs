namespace LifeCardGame.Core.Data;
[SingletonGame]
public class LifeCardGameDetailClass : IGameInfo, ICardInfo<LifeCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    string IGameInfo.GameName => "Life Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<LifeCardGameCardInformation>.CardsToPassOut => 5;
    BasicList<int> ICardInfo<LifeCardGameCardInformation>.PlayerExcludeList
    {
        get
        {
            LifeCardGameGameContainer gameContainer = aa.Resolver!.Resolve<LifeCardGameGameContainer>();
            return gameContainer.YearCards().Select(items => items.Deck).ToBasicList();
        }
    }
    bool ICardInfo<LifeCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<LifeCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<LifeCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<LifeCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<LifeCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<LifeCardGameCardInformation>.NoPass => false;
    bool ICardInfo<LifeCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<LifeCardGameCardInformation> ICardInfo<LifeCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<LifeCardGameCardInformation>();
    bool ICardInfo<LifeCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<LifeCardGameCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<LifeCardGameCardInformation>.DiscardExcludeList(IListShuffler<LifeCardGameCardInformation> deckList)
    {
        return new();
    }
}