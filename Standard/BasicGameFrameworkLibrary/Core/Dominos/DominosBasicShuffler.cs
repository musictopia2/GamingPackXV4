﻿namespace BasicGameFrameworkLibrary.Core.Dominos;
public class DominosBasicShuffler<D> : IDeckShuffler<D>, IScatterList<D>,
     IAdvancedDIContainer, ISimpleList<D>, IListShuffler<D> where D : IDominoInfo, new()
{
    private readonly BasicObjectShuffler<D> _thisShuffle;
    private readonly DeckRegularDict<D> _objectList = new();
    public int Count => _objectList.Count;
    public DominosBasicShuffler()
    {
        _thisShuffle = new BasicObjectShuffler<D>(_objectList); //i think.
    }
    public IGamePackageResolver? MainContainer { get => _thisShuffle.MainContainer; set => _thisShuffle.MainContainer = value; }
    public bool NeedsToRedo { get => _thisShuffle.NeedsToRedo; set => _thisShuffle.NeedsToRedo = value; }
    public IGamePackageGeneratorDI? GeneratorContainer { get => _thisShuffle.GeneratorContainer; set => _thisShuffle.GeneratorContainer = value; }
    public void ClearObjects()
    {
        _thisShuffle.ClearObjects();
    }
    public bool Contains(D item)
    {
        return _objectList.Contains(item);
    }
    public bool Exists(Predicate<D> match)
    {
        return _objectList.Exists(match);
    }
    public D Find(Predicate<D> match)
    {
        return _objectList.Find(match)!;
    }
    public IBasicList<D> FindAll(Predicate<D> match)
    {
        return _objectList.FindAll(match);
    }
    public int FindFirstIndex(Predicate<D> match)
    {
        return _objectList.FindFirstIndex(match);
    }
    public int FindFirstIndex(int startIndex, Predicate<D> match)
    {
        return _objectList.FindFirstIndex(startIndex, match);
    }
    public int FindFirstIndex(int startIndex, int count, Predicate<D> match)
    {
        return _objectList.FindFirstIndex(startIndex, count, match);
    }
    public D FindLast(Predicate<D> match)
    {
        return _objectList.FindLast(match)!;
    }
    public int FindLastIndex(Predicate<D> match)
    {
        return _objectList.FindLastIndex(match);
    }
    public int FindLastIndex(int startIndex, Predicate<D> match)
    {
        return _objectList.FindLastIndex(startIndex, match);
    }
    public int FindLastIndex(int startIndex, int count, Predicate<D> match)
    {
        return _objectList.FindLastIndex(startIndex, count, match);
    }
    public D FindOnlyOne(Predicate<D> match)
    {
        return _objectList.FindOnlyOne(match);
    }
    public void ForConditionalItems(Predicate<D> match, Action<D> action)
    {
        _objectList.ForConditionalItems(match, action);
    }
    public Task ForConditionalItemsAsync(Predicate<D> match, ActionAsync<D> action)
    {
        return _objectList.ForConditionalItemsAsync(match, action);
    }
    public void ForEach(Action<D> action)
    {
        _objectList.ForEach(action);
    }
    public Task ForEachAsync(ActionAsync<D> action)
    {
        return _objectList.ForEachAsync(action);
    }
    public int GetDeckCount()
    {
        return _thisShuffle.GetDeckCount();
    }
    public IEnumerator<D> GetEnumerator()
    {
        return _objectList.GetEnumerator();
    }
    public Task<DeckRegularDict<D>> GetListFromJsonAsync(string jsonData)
    {
        return _thisShuffle.GetListFromJsonAsync(jsonData);
    }
    public D GetSpecificItem(int deck)
    {
        return _thisShuffle.GetSpecificItem(deck);
    }
    public int HowMany(Predicate<D> match)
    {
        return _objectList.HowMany(match);
    }
    public int IndexOf(D value)
    {
        return _objectList.IndexOf(value);
    }
    public int IndexOf(D value, int Index)
    {
        return _objectList.IndexOf(value, Index);
    }
    public int IndexOf(D value, int Index, int Count)
    {
        return _objectList.IndexOf(value, Index, Count);
    }
    public bool ObjectExist(int deck)
    {
        return _thisShuffle.ObjectExist(deck);
    }
    public void OrderedObjects()
    {
        _thisShuffle.OrderedObjects();
    }
    public void ReshuffleFirstObjects(IDeckDict<D> thisList, int startAt, int endAt)
    {
        _thisShuffle.ReshuffleFirstObjects(thisList, startAt, endAt);
    }
    public void ShuffleObjects()
    {
        _thisShuffle.ShuffleObjects();
    }
    public bool TrueForAll(Predicate<D> match)
    {
        return _objectList.TrueForAll(match);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _objectList.GetEnumerator();
    }
    public void RelinkObject(int oldDeck, D newObject)
    {
        _thisShuffle.RelinkObject(oldDeck, newObject);
    }
    public void PutCardOnTop(int deck)
    {
        _thisShuffle.PutCardOnTop(deck);
    }
}