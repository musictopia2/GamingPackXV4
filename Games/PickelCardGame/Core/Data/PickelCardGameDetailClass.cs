namespace PickelCardGame.Core.Data;
[SingletonGame]
public class PickelCardGameDetailClass : IGameInfo, ICardInfo<PickelCardGameCardInformation>, ITrickData,
    IBeginningRegularCards<PickelCardGameCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Pickel Card Game";
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
    int ICardInfo<PickelCardGameCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<PickelCardGameCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<PickelCardGameCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<PickelCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<PickelCardGameCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<PickelCardGameCardInformation>.PassOutAll => false;
    bool ICardInfo<PickelCardGameCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<PickelCardGameCardInformation>.NoPass => false;
    bool ICardInfo<PickelCardGameCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<PickelCardGameCardInformation> ICardInfo<PickelCardGameCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<PickelCardGameCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<PickelCardGameCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => true;
    bool ITrickData.HasTrump => true;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<PickelCardGameCardInformation>.DiscardExcludeList(IListShuffler<PickelCardGameCardInformation> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<PickelCardGameCardInformation>.AceLow => false;
    bool IBeginningRegularCards<PickelCardGameCardInformation>.CustomDeck => true;
}