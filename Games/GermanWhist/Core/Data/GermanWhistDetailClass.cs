namespace GermanWhist.Core.Data;
[SingletonGame]
public class GermanWhistDetailClass : IGameInfo, ICardInfo<GermanWhistCardInformation>, ITrickData,
    IBeginningRegularCards<GermanWhistCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "German Whist";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 2;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<GermanWhistCardInformation>.CardsToPassOut => 13;
    BasicList<int> ICardInfo<GermanWhistCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<GermanWhistCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<GermanWhistCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<GermanWhistCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<GermanWhistCardInformation>.PassOutAll => false;
    bool ICardInfo<GermanWhistCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<GermanWhistCardInformation>.NoPass => false;
    bool ICardInfo<GermanWhistCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<GermanWhistCardInformation> ICardInfo<GermanWhistCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<GermanWhistCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<GermanWhistCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<GermanWhistCardInformation>.DiscardExcludeList(IListShuffler<GermanWhistCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<GermanWhistCardInformation>.AceLow => false;
    bool IBeginningRegularCards<GermanWhistCardInformation>.CustomDeck => false;
}