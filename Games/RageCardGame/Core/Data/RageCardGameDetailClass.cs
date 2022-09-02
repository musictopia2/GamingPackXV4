namespace RageCardGame.Core.Data;
[SingletonGame]
public class RageCardGameDetailClass : IGameInfo, ICardInfo<RageCardGameCardInformation>, ITrickData
{
    private readonly RageDelgates _delgates;
    public RageCardGameDetailClass(RageDelgates delgates)
    {
        _delgates = delgates;
    }
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => true;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Rage Card Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 8;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<RageCardGameCardInformation>.CardsToPassOut => _delgates.CardsToPassOut!.Invoke();
    BasicList<int> ICardInfo<RageCardGameCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<RageCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<RageCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<RageCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<RageCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<RageCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<RageCardGameCardInformation>.NoPass => false;
    bool ICardInfo<RageCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<RageCardGameCardInformation> ICardInfo<RageCardGameCardInformation>.DummyHand { get; set; } = new DeckRegularDict<RageCardGameCardInformation>();
    bool ICardInfo<RageCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<RageCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<RageCardGameCardInformation>.DiscardExcludeList(IListShuffler<RageCardGameCardInformation> deckList)
    {
        return new();
    }
}