namespace Flinch.Core.Data;
[SingletonGame]
public class FlinchDetailClass : IGameInfo, ICardInfo<FlinchCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = true;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Flinch";
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
    int ICardInfo<FlinchCardInformation>.CardsToPassOut => 10;
    BasicList<int> ICardInfo<FlinchCardInformation>.PlayerExcludeList => [];
    bool ICardInfo<FlinchCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<FlinchCardInformation>.ReshuffleAllCardsFromDiscard => true;
    bool ICardInfo<FlinchCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<FlinchCardInformation>.PassOutAll => false;
    bool ICardInfo<FlinchCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<FlinchCardInformation>.NoPass => false;
    bool ICardInfo<FlinchCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<FlinchCardInformation> ICardInfo<FlinchCardInformation>.DummyHand { get; set; } = [];
    bool ICardInfo<FlinchCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<FlinchCardInformation>.CanSortCardsToBeginWith => false;
    BasicList<int> ICardInfo<FlinchCardInformation>.DiscardExcludeList(IListShuffler<FlinchCardInformation> deckList)
    {
        return [];
    }
}