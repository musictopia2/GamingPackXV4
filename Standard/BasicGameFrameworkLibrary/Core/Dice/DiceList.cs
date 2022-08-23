﻿namespace BasicGameFrameworkLibrary.Core.Dice;

public class DiceList<D> : ISimpleList<D>
    where D : IStandardDice, new()
{
    private readonly BasicList<D> _privateList = new();
    public D this[int index]
    {
        get { return _privateList[index]; }
    }
    public int GetDiceIndex(D thisDice)
    {
        return _privateList.IndexOf(thisDice);
    }
    public void SortDice()
    {
        SortDice(false);
    }
    public void ReplaceDiceRange(ISimpleList<D> thisList) //needs to be public so  i can use for testing.
    {
        _privateList.ReplaceRange(thisList);
    }
    public BasicList<D> GetSelectedItems() => _privateList.Where(items => items.IsSelected).ToBasicList();
    internal void RemoveSelectedDice()
    {
        _privateList.RemoveAllOnly(items => items.IsSelected == true);
    }
    internal void RemoveConditionalDice(Predicate<D> predicate)
    {
        _privateList.RemoveAllOnly(predicate);
    }
    public void SortDice(bool descending)
    {
        _thisSort.IsDescending = descending;
        _privateList.Sort(comparer: _thisSort);
    }
    public DiceList() { }
    public DiceList(IEnumerable<D> tempList)
    {
        _privateList.ReplaceRange(tempList); // so i keep the linking.
    }
    public void Clear(int howMany)
    {
        SetContainer(); //i think here too.
        _privateList.ReplaceWithNewObjects(howMany, () => new D()); //if i don't do it this way, then i have breaking changes.
    }
    public IGamePackageResolver? MainContainer { get; set; }
    public int Count => _privateList.Count;
    private readonly SortBasicDice<D> _thisSort = new(); //this for sure needs it.
    private void SetContainer()
    {
        if (MainContainer != null)
        {
            return;
        }
        throw new CustomBasicException("Container not set.  Rethink");
    }
    public D GetRandomDice(int upTo)
    {
        SetContainer();
        IDiceContainer<int> thisG = MainContainer!.Resolve<IDiceContainer<int>>();
        thisG.MainContainer = MainContainer;
        BasicList<int> possList = thisG.GetPossibleList;
        int Chosen = possList.GetRandomItem();
        D newDice = new();
        newDice.Populate(Chosen);
        newDice.Index = upTo;
        return newDice;
    }
    public Task ForEachAsync(ActionAsync<D> action)
    {
        return _privateList.ForEachAsync(action);
    }
    public void ForEach(Action<D> action)
    {
        _privateList.ForEach(action);
    }
    public void ForConditionalItems(Predicate<D> match, Action<D> action)
    {
        _privateList.ForConditionalItems(match, action);
    }
    public Task ForConditionalItemsAsync(Predicate<D> match, ActionAsync<D> action)
    {
        return _privateList.ForConditionalItemsAsync(match, action);
    }
    public bool Exists(Predicate<D> match)
    {
        return _privateList.Exists(match);
    }
    public bool Contains(D item)
    {
        return _privateList.Contains(item);
    }
    public D Find(Predicate<D> match)
    {
        return _privateList.Find(match)!;
    }
    public D FindOnlyOne(Predicate<D> match)
    {
        return _privateList.FindOnlyOne(match);
    }
    public IBasicList<D> FindAll(Predicate<D> match)
    {
        return _privateList.FindAll(match);
    }
    public int FindFirstIndex(Predicate<D> match)
    {
        return _privateList.FindFirstIndex(match);
    }
    public int FindFirstIndex(int startIndex, Predicate<D> match)
    {
        return _privateList.FindFirstIndex(startIndex, match);
    }
    public int FindFirstIndex(int startIndex, int count, Predicate<D> match)
    {
        return _privateList.FindFirstIndex(startIndex, count, match);
    }
    public D FindLast(Predicate<D> match)
    {
        return _privateList.FindLast(match)!;
    }
    public int FindLastIndex(Predicate<D> match)
    {
        return _privateList.FindLastIndex(match);
    }
    public int FindLastIndex(int startIndex, Predicate<D> match)
    {
        return _privateList.FindLastIndex(startIndex, match);
    }
    public int FindLastIndex(int startIndex, int count, Predicate<D> match)
    {
        return _privateList.FindLastIndex(startIndex, count, match);
    }
    public int HowMany(Predicate<D> match)
    {
        return _privateList.HowMany(match);
    }
    public int IndexOf(D value)
    {
        return _privateList.IndexOf(value);
    }
    public int IndexOf(D value, int Index)
    {
        return _privateList.IndexOf(value, Index);
    }
    public int IndexOf(D value, int Index, int Count)
    {
        return _privateList.IndexOf(value, Index, Count);
    }
    public bool TrueForAll(Predicate<D> match)
    {
        return _privateList.TrueForAll(match);
    }
    public IEnumerator<D> GetEnumerator()
    {
        return _privateList.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _privateList.GetEnumerator();
    }
}