namespace Cribbage.Core.Data;
[SingletonGame]
public class CribbageDetailClass(CribbageDelegates delegates) : IGameInfo, ICardInfo<CribbageCard>,
    IBeginningRegularCards<CribbageCard>
{
    EnumGameType IGameInfo.GameType => EnumGameType.Rounds;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Cribbage";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 3;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
    int ICardInfo<CribbageCard>.CardsToPassOut
    {
        get
        {
            if (delegates.GetPlayerCount == null)
            {
                throw new CustomBasicException("Nobody is handling get player count.  Rethink");
            }
            int count = delegates.GetPlayerCount.Invoke();
            if (count == 2)
            {
                return 6;
            }
            else if (count == 3)
            {
                return 5;
            }
            else
            {
                throw new CustomBasicException("Only 2 or 3 players are supported");
            }
        }
    }
    BasicList<int> ICardInfo<CribbageCard>.PlayerExcludeList => [];
    bool ICardInfo<CribbageCard>.AddToDiscardAtBeginning => true;
    bool ICardInfo<CribbageCard>.ReshuffleAllCardsFromDiscard => false;
    bool ICardInfo<CribbageCard>.ShowMessageWhenReshuffling => true;
    bool ICardInfo<CribbageCard>.PassOutAll => false;
    bool ICardInfo<CribbageCard>.PlayerGetsCards => true;
    bool ICardInfo<CribbageCard>.NoPass => false;
    bool ICardInfo<CribbageCard>.NeedsDummyHand => false;
    DeckRegularDict<CribbageCard> ICardInfo<CribbageCard>.DummyHand { get; set; } = [];
    bool ICardInfo<CribbageCard>.HasDrawAnimation => true;
    bool ICardInfo<CribbageCard>.CanSortCardsToBeginWith => true;
    BasicList<int> ICardInfo<CribbageCard>.DiscardExcludeList(IListShuffler<CribbageCard> deckList)
    {
        return [];
    }
    bool IBeginningRegularCards<CribbageCard>.AceLow => true;
    bool IBeginningRegularCards<CribbageCard>.CustomDeck => false;
}