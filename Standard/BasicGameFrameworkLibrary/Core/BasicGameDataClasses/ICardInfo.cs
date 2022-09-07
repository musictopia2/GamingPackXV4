namespace BasicGameFrameworkLibrary.Core.BasicGameDataClasses;
/// <summary>
/// this is used for card games.  this is all the data that has to be populated to change behaviors based on information.
/// </summary>
public interface ICardInfo<D> where D : IDeckObject, new()
{
    int CardsToPassOut { get; }
    BasicList<int> DiscardExcludeList(IListShuffler<D> deckList);
    BasicList<int> PlayerExcludeList { get; }
    bool AddToDiscardAtBeginning { get; }
    bool ReshuffleAllCardsFromDiscard { get; }
    bool ShowMessageWhenReshuffling { get; }
    bool PassOutAll { get; }
    bool PlayerGetsCards { get; }
    bool NoPass { get; }
    bool NeedsDummyHand { get; }
    DeckRegularDict<D> DummyHand { get; set; }
    bool HasDrawAnimation { get; }
    bool CanSortCardsToBeginWith { get; }
}