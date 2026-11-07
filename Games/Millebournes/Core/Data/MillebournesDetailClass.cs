namespace Millebournes.Core.Data;
[SingletonGame]
public class MillebournesDetailClass : IGameInfo, ICardInfo<MillebournesCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    public const bool CanHaveExtraComputerPlayers = true;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Millebournes";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 5;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 6;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<MillebournesCardInformation>.CardsToPassOut => 6;
    BasicList<int> ICardInfo<MillebournesCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<MillebournesCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<MillebournesCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<MillebournesCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<MillebournesCardInformation>.PassOutAll => false;
    bool ICardInfo<MillebournesCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<MillebournesCardInformation>.NoPass => false;
    bool ICardInfo<MillebournesCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<MillebournesCardInformation> ICardInfo<MillebournesCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<MillebournesCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<MillebournesCardInformation>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<MillebournesCardInformation>.DiscardExcludeList(IListShuffler<MillebournesCardInformation> deckList)
    {
        return [];
    }
}