namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Dictionary;
public interface IDeckDict<D> : IBasicList<D>, IDeckLookUp<D> where D : IDeckObject
{
    D RemoveObjectByDeck(int deck);
}