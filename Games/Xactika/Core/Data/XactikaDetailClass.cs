namespace Xactika.Core.Data;
[SingletonGame]
public class XactikaDetailClass : IGameInfo, ICardInfo<XactikaCardInformation>, ITrickData
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Xactika";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<XactikaCardInformation>.CardsToPassOut => 8;
    BasicList<int> ICardInfo<XactikaCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<XactikaCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<XactikaCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<XactikaCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<XactikaCardInformation>.PassOutAll => false;
    bool ICardInfo<XactikaCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<XactikaCardInformation>.NoPass => false;
    bool ICardInfo<XactikaCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<XactikaCardInformation> ICardInfo<XactikaCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<XactikaCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<XactikaCardInformation>.CanSortCardsToBeginWith => true;
    bool ITrickData.FirstPlayerAnySuit => true;
    bool ITrickData.FollowSuit => true;
    bool ITrickData.MustFollow => false;
    bool ITrickData.HasTrump => false;
    bool ITrickData.MustPlayTrump => false;
    EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;
    bool ITrickData.HasDummy => false;
    BasicList<int> ICardInfo<XactikaCardInformation>.DiscardExcludeList(IListShuffler<XactikaCardInformation> deckList)
    {
        return [];
    }
}