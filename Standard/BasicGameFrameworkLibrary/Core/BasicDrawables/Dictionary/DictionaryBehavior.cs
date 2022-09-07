﻿namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Dictionary;
internal class DictionaryBehavior<D> : IListModifiers<D> where D : IDeckObject
{
    private Dictionary<int, D> _privateDict = new();
    public D SearchItem(int deck)
    {
        return _privateDict[deck];
    }
    public bool ObjectExist(int deck)
    {
        return _privateDict.ContainsKey(deck);
    }
    public void ReplaceDictionaryValue(int oldValue, int newID, D newValue)
    {
        _privateDict.Remove(oldValue);
        _privateDict.Add(newID, newValue);
    }
    public void Add(D value)
    {
        if (value.Deck <= 0)
        {
            throw new CustomBasicException("Deck cannot be 0");
        }
        _privateDict.Add(value.Deck, value);
    }
    public void AddRange(IEnumerable<D> thisList)
    {
        foreach (var item in thisList)
        {
            Add(item); //so it goes through that error handling.
        }
    }
    public void Clear()
    {
        _privateDict.Clear();
    }
    public void LoadStartLists(IEnumerable<D> thisList)
    {
        _privateDict = thisList.ToDictionary(Items => Items.Deck);
    }
    public bool RemoveSpecificItem(D value)
    {
        return _privateDict.Remove(value.Deck);
    }
    public void ReplaceAllWithGivenItem(D value)
    {
        _privateDict.Clear();
        _privateDict.Add(value.Deck, value);
    }
    public void ReplaceItem(D oldItem, D newItem)
    {
        bool rets;
        rets = _privateDict.Remove(oldItem.Deck);
        if (rets == false)
        {
            throw new CustomBasicException($"{oldItem.Deck} from old did not exist");
        }
        _privateDict.Add(newItem.Deck, newItem);
    }
    public void ReplaceRange(IEnumerable<D> thisList)
    {
        _privateDict = thisList.ToDictionary(Items => Items.Deck);
    }
}