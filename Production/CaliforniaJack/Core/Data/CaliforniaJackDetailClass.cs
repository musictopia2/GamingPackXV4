namespace CaliforniaJack.Core.Data;
[SingletonGame]
public class CaliforniaJackDetailClass : IGameInfo, ICardInfo<CaliforniaJackCardInformation>, ITrickData,
    IBeginningRegularCards<CaliforniaJackCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "California Jack";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<CaliforniaJackCardInformation>.CardsToPassOut => 6;
    BasicList<int> ICardInfo<CaliforniaJackCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<CaliforniaJackCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<CaliforniaJackCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<CaliforniaJackCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<CaliforniaJackCardInformation>.PassOutAll => false;
    bool ICardInfo<CaliforniaJackCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<CaliforniaJackCardInformation>.NoPass => false;
    bool ICardInfo<CaliforniaJackCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<CaliforniaJackCardInformation> ICardInfo<CaliforniaJackCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<CaliforniaJackCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<CaliforniaJackCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => true;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<CaliforniaJackCardInformation>.DiscardExcludeList(IListShuffler<CaliforniaJackCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<CaliforniaJackCardInformation>.AceLow => false;
    bool IBeginningRegularCards<CaliforniaJackCardInformation>.CustomDeck => false;
}