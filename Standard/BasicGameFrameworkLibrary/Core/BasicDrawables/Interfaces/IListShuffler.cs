namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;
public interface IListShuffler<D> : IDeckShuffler<D>, ISimpleList<D>, IEnumerableDeck<D> where D : IDeckObject, new()
{
    void RelinkObject(int oldDeck, D newObject);
}