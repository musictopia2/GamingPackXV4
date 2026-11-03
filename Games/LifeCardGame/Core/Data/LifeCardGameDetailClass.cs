namespace LifeCardGame.Core.Data;
[SingletonGame]
public class LifeCardGameDetailClass : IGameInfo, ICardInfo<LifeCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    public const string GameName = "Life Card Game";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<LifeCardGameCardInformation>.CardsToPassOut => 5;
    BasicList<int> ICardInfo<LifeCardGameCardInformation>.PlayerExcludeList
    {
        get
        {
            LifeCardGameGameContainer gameContainer = Resolver!.Resolve<LifeCardGameGameContainer>();
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
    DeckRegularDict<LifeCardGameCardInformation> ICardInfo<LifeCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<LifeCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<LifeCardGameCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<LifeCardGameCardInformation>.DiscardExcludeList(IListShuffler<LifeCardGameCardInformation> deckList)
    {
        return [];
    }
}