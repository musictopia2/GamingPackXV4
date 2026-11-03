namespace Millebournes.Core.Data;
[SingletonGame]
public class MillebournesDetailClass : IGameInfo, ICardInfo<MillebournesCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => true;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "Millebournes";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 5;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
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