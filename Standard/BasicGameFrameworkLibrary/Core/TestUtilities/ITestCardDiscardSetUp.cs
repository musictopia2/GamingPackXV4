namespace BasicGameFrameworkLibrary.Core.TestUtilities;
public interface ITestCardDiscardSetUp<D>
    where D : IDeckObject, new()
{
    D GetFirstCardForDiscardPile(IListShuffler<D> deckList);
}