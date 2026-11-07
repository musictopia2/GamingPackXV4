namespace Hearts.Core.Data;
[SingletonGame]
public class HeartsDetailClass : IGameInfo, ICardInfo<HeartsCardInformation>, ITrickData,
    IBeginningRegularCards<HeartsCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = true;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers; //i think.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType; //the computer can't skip turns. otherwise, gets stuck.
    public const string GameName = "Hearts";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 4;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<HeartsCardInformation>.CardsToPassOut => 13; //change to what you need.
    BasicList<int> ICardInfo<HeartsCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<HeartsCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<HeartsCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<HeartsCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<HeartsCardInformation>.PassOutAll => false;
    bool ICardInfo<HeartsCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<HeartsCardInformation>.NoPass => false;
    bool ICardInfo<HeartsCardInformation>.NeedsDummyHand => false; //no dummy hand.
    DeckRegularDict<HeartsCardInformation> ICardInfo<HeartsCardInformation>.DummyHand { get; set; } = [];
    BasicList<int> ICardInfo<HeartsCardInformation>.DiscardExcludeList(IListShuffler<HeartsCardInformation> deckList)
    {
        return [];
    }
    bool ICardInfo<HeartsCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<HeartsCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.Hearts;
    bool ITrickData.HasDummy => false;
    bool IBeginningRegularCards<HeartsCardInformation>.AceLow => false;
    bool IBeginningRegularCards<HeartsCardInformation>.CustomDeck => false;
}