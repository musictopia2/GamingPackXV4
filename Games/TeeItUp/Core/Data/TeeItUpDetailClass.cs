namespace TeeItUp.Core.Data;
[SingletonGame]
public class TeeItUpDetailClass : IGameInfo, ICardInfo<TeeItUpCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Tee It Up";
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
    int ICardInfo<TeeItUpCardInformation>.CardsToPassOut => 8;
    BasicList<int> ICardInfo<TeeItUpCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<TeeItUpCardInformation>.AddToDiscardAtBeginning => true;
    bool ICardInfo<TeeItUpCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<TeeItUpCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<TeeItUpCardInformation>.PassOutAll => false;
    bool ICardInfo<TeeItUpCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<TeeItUpCardInformation>.NoPass => false;
    bool ICardInfo<TeeItUpCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<TeeItUpCardInformation> ICardInfo<TeeItUpCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<TeeItUpCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<TeeItUpCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<TeeItUpCardInformation>.DiscardExcludeList(IListShuffler<TeeItUpCardInformation> deckList)
    {
        return [];
    }
}