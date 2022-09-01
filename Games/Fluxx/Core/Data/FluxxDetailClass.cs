namespace Fluxx.Core.Data;
[SingletonGame]
public class FluxxDetailClass : IGameInfo, ICardInfo<FluxxCardInformation>, INewCard<FluxxCardInformation>
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Fluxx";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4; //originally was 6 but had to reduce to 4.  otherwise, not enough room.
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.LargeDevices;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<FluxxCardInformation>.CardsToPassOut => 3;
    BasicList<int> ICardInfo<FluxxCardInformation>.PlayerExcludeList => new() { 1 };
    bool ICardInfo<FluxxCardInformation>.AddToDiscardAtBeginning => false;
    bool ICardInfo<FluxxCardInformation>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<FluxxCardInformation>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<FluxxCardInformation>.PassOutAll => false;
    bool ICardInfo<FluxxCardInformation>.PlayerGetsCards => true;
    bool ICardInfo<FluxxCardInformation>.NoPass => false;
    bool ICardInfo<FluxxCardInformation>.NeedsDummyHand => false;
    DeckRegularDict<FluxxCardInformation> ICardInfo<FluxxCardInformation>.DummyHand { get; set; } = new DeckRegularDict<FluxxCardInformation>();
    bool ICardInfo<FluxxCardInformation>.HasDrawAnimation => true;
    bool ICardInfo<FluxxCardInformation>.CanSortCardsToBeginWith => true;
    FluxxCardInformation INewCard<FluxxCardInformation>.GetNewCard(int chosen)
    {
        return GetNewCard(chosen);
    }
    public static FluxxCardInformation GetNewCard(int chosen)
    {
        if (chosen <= 0)
        {
            throw new CustomBasicException("Must choose a value above 0 for GetNewCard for Fluxx");
        }
        if (chosen <= 22)
        {
            return new RuleCard();
        }
        if (chosen <= 40)
        {
            return new KeeperCard();
        }
        if (chosen <= 63)
        {
            return new GoalCard();
        }
        if (chosen <= 83)
        {
            return new ActionCard();
        }
        throw new CustomBasicException("Must go only up to 83");
    }
    BasicList<int> ICardInfo<FluxxCardInformation>.DiscardExcludeList(IListShuffler<FluxxCardInformation> deckList)
    {
        return new();
    }
}