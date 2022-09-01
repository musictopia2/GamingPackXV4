namespace GalaxyCardGame.Core.Data;
[SingletonGame]
public class GalaxyCardGameDetailClass : IGameInfo, ICardInfo<GalaxyCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<GalaxyCardGameCardInformation>
{
    private readonly GalaxyDelegates _delegates;
    public GalaxyCardGameDetailClass(GalaxyDelegates delegates)
    {
        _delegates = delegates;
    }
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Galaxy Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<GalaxyCardGameCardInformation>.CardsToPassOut => 9;
    BasicList<int> ICardInfo<GalaxyCardGameCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<GalaxyCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<GalaxyCardGameCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<GalaxyCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<GalaxyCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<GalaxyCardGameCardInformation>.PlayerGetsCards => _delegates.PlayerGetCards!.Invoke();
    bool ICardInfo<GalaxyCardGameCardInformation>.NoPass => false;
    bool ICardInfo<GalaxyCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<GalaxyCardGameCardInformation> ICardInfo<GalaxyCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<GalaxyCardGameCardInformation>();
    bool ICardInfo<GalaxyCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<GalaxyCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => false;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<GalaxyCardGameCardInformation>.DiscardExcludeList(IListShuffler<GalaxyCardGameCardInformation> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<GalaxyCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<GalaxyCardGameCardInformation>.CustomDeck => false;
}