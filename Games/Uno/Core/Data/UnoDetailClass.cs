namespace Uno.Core.Data;
[SingletonGame]
public class UnoDetailClass : IGameInfo, ICardInfo<UnoCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = true;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Uno";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 6;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<UnoCardInformation>.CardsToPassOut => 7;
    BasicList<int> ICardInfo<UnoCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<UnoCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<UnoCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<UnoCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<UnoCardInformation>.PassOutAll => false;
    bool ICardInfo<UnoCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<UnoCardInformation>.NoPass => false;
    bool ICardInfo<UnoCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<UnoCardInformation> ICardInfo<UnoCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<UnoCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<UnoCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<UnoCardInformation>.DiscardExcludeList(IListShuffler<UnoCardInformation> deckList)
    {
        return deckList.Where(x => x.WhichType == EnumCardTypeList.Wild && x.Draw == 4).Select(x => x.Deck).ToBasicList();
    }
}