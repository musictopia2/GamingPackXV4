namespace FillOrBust.Core.Data;
[SingletonGame]
public class FillOrBustDetailClass : IGameInfo, ICardInfo<FillOrBustCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "Fill Or Bust";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 8;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int ICardInfo<FillOrBustCardInformation>.CardsToPassOut => 0;
    BasicList<int> ICardInfo<FillOrBustCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<FillOrBustCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<FillOrBustCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<FillOrBustCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<FillOrBustCardInformation>.PassOutAll => false;
    bool ICardInfo<FillOrBustCardInformation>.PlayerGetsCards => false;
    bool ICardInfo<FillOrBustCardInformation>.NoPass => true;
    bool ICardInfo<FillOrBustCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<FillOrBustCardInformation> ICardInfo<FillOrBustCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<FillOrBustCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<FillOrBustCardInformation>.CanSortCardsToBeginWith => false;
    BasicList<int> ICardInfo<FillOrBustCardInformation>.DiscardExcludeList(IListShuffler<FillOrBustCardInformation> deckList)
    {
        return [];
    }
}