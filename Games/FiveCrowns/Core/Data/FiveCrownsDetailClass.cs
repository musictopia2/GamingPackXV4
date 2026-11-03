namespace FiveCrowns.Core.Data;
[SingletonGame]
public class FiveCrownsDetailClass(FiveCrownsDelegates delegates) : IGameInfo, ICardInfo<FiveCrownsCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;

    bool IGameInfo.CanHaveExtraComputerPlayers => false;

    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;

    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

    public const string GameName = "Five Crowns";
    string IGameInfo.GameName => GameName;

    int IGameInfo.NoPlayers => 0;

    int IGameInfo.MinPlayers => 2;

    int IGameInfo.MaxPlayers => 7;

    bool IGameInfo.CanAutoSave => true;

    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;


    int ICardInfo<FiveCrownsCardInformation>.CardsToPassOut => delegates.CardsToPassOut!.Invoke();

    BasicList<int> ICardInfo<FiveCrownsCardInformation>.PlayerExcludeList => [];

    bool ICardInfo<FiveCrownsCardInformation>.AddToDiscardAtBeginning => true;

    bool ICardInfo<FiveCrownsCardInformation>.ReshuffleAllCardsFromDiscard => false;

    bool ICardInfo<FiveCrownsCardInformation>.ShowMessageWhenReshuffling => true;

    bool ICardInfo<FiveCrownsCardInformation>.PassOutAll => false;

    bool ICardInfo<FiveCrownsCardInformation>.PlayerGetsCards => true;

    bool ICardInfo<FiveCrownsCardInformation>.NoPass => false;

    bool ICardInfo<FiveCrownsCardInformation>.NeedsDummyHand => false;

    DeckRegularDict<FiveCrownsCardInformation> ICardInfo<FiveCrownsCardInformation>.DummyHand { get; set; } = [];

    bool ICardInfo<FiveCrownsCardInformation>.HasDrawAnimation => true;

    bool ICardInfo<FiveCrownsCardInformation>.CanSortCardsToBeginWith => true;

    BasicList<int> ICardInfo<FiveCrownsCardInformation>.DiscardExcludeList(IListShuffler<FiveCrownsCardInformation> deckList)
    {
        return [];
    }
}