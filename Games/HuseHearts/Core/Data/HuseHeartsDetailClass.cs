namespace HuseHearts.Core.Data;
[SingletonGame]
public class HuseHeartsDetailClass(HuseHeartsDelegates delegates) : IGameInfo, ICardInfo<HuseHeartsCardInformation>, ITrickData,
    IBeginningRegularCards<HuseHeartsCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Huse Hearts";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 2;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<HuseHeartsCardInformation>.CardsToPassOut => 16;
    BasicList<int> ICardInfo<HuseHeartsCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<HuseHeartsCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<HuseHeartsCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<HuseHeartsCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<HuseHeartsCardInformation>.PassOutAll => false;
    bool ICardInfo<HuseHeartsCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<HuseHeartsCardInformation>.NoPass => false;
    bool ICardInfo<HuseHeartsCardInformation>.NeedsDummyHand => true;
    DeckRegularDict<HuseHeartsCardInformation> ICardInfo<HuseHeartsCardInformation>.DummyHand
    {
        get => delegates.GetDummyList!.Invoke();
        set => delegates.SetDummyList!(value);
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
        return [];
    }
    bool IBeginningRegularCards<HuseHeartsCardInformation>.AceLow => false;
    bool IBeginningRegularCards<HuseHeartsCardInformation>.CustomDeck => false;
}