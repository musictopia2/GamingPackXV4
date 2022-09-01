namespace HuseHearts.Core.Data;
[SingletonGame]
public class HuseHeartsDetailClass : IGameInfo, ICardInfo<HuseHeartsCardInformation>, ITrickData,
    IBeginningRegularCards<HuseHeartsCardInformation>
{
    private readonly HuseHeartsDelegates _delegates;
    public HuseHeartsDetailClass(HuseHeartsDelegates delegates)
    {
        _delegates = delegates;
    }
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Huse Hearts";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<HuseHeartsCardInformation>.CardsToPassOut => 16;
    BasicList<int> ICardInfo<HuseHeartsCardInformation>.PlayerExcludeList => new();
    bool ICardInfo<HuseHeartsCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<HuseHeartsCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<HuseHeartsCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<HuseHeartsCardInformation>.PassOutAll => false;
    bool ICardInfo<HuseHeartsCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<HuseHeartsCardInformation>.NoPass => false;
    bool ICardInfo<HuseHeartsCardInformation>.NeedsDummyHand => true;
    DeckRegularDict<HuseHeartsCardInformation> ICardInfo<HuseHeartsCardInformation>.DummyHand
    {
        get => _delegates.GetDummyList!.Invoke();
        set => _delegates.SetDummyList!(value);
    }
    bool ICardInfo<HuseHeartsCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<HuseHeartsCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.Hearts;
    bool ITrickData.HasDummy => true;
    BasicList<int> ICardInfo<HuseHeartsCardInformation>.DiscardExcludeList(IListShuffler<HuseHeartsCardInformation> deckList)
    {
        return new();
    }
    bool IBeginningRegularCards<HuseHeartsCardInformation>.AceLow => false;
    bool IBeginningRegularCards<HuseHeartsCardInformation>.CustomDeck => false;
}