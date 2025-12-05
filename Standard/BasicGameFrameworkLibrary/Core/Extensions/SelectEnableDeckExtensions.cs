namespace BasicGameFrameworkLibrary.Core.Extensions;
public static class SelectEnableDeckExtensions
{
    extension(string data)
    {
        public async Task<BasicList<int>> GetSavedIntegerListAsync()
        {
            return await js1.DeserializeObjectAsync<BasicList<int>>(data);
        }
        public async Task<DeckRegularDict<D>> GetNewObjectListFromDeckListAsync<D>(IDeckLookUp<D> thisBase) where D : IDeckObject
        {
            DeckRegularDict<D> output = new();
            BasicList<int> thisList = await js1.DeserializeObjectAsync<BasicList<int>>(data);
            thisList.ForEach(items =>
            {
                output.Add(thisBase.GetSpecificItem(items));
            });
            return output;
        }
    }
    extension <E>(BasicPickerData<E> piece)
        where E : struct, IFastEnumColorSimple
    {
        public string ColorFromEnum => piece.EnumValue.Color;
    }
    extension (ISelectableObject thisItem)
    {
        public void SelectUnselectItem()
        {
            thisItem.IsSelected = !thisItem.IsSelected;
        }
    }
    extension <S>(ISimpleList<S> list)
        where S : ISelectableObject
    {
        public void SelectUnselectItem(int index) //has to be generics or casting problems.
        {
            int i = 0;
            foreach (var item in list)
            {
                if (i == index)
                {
                    item.SelectUnselectItem();
                    return;
                }
                i++;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        public void SelectSpecificItem<V>(Func<S, V> selector, V value)
        {
            list.ForEach(items =>
            {
                if (selector(items)!.Equals(value)) //has to be this way still.
                {
                    items.IsSelected = true;
                }
                else
                {
                    items.IsSelected = false;
                }
            });
        }
        public void SelectSeveralItems<V>(Func<S, V> selector,
            ISimpleList<V> valuelist)
        {
            list.UnselectAllObjects();

            valuelist.ForEach(value =>
            {
                list.ForEach(items =>
                {
                    if (selector(items)!.Equals(value))
                    {
                        items.IsSelected = true;
                    }
                });
            });
        }
        public BasicList<S> GetSelectedItems()
            => list.Where(items =>
            items.IsSelected == true).ToBasicList();
        public int HowManySelectedItems => list.Count(items => items.IsSelected);
        public void UnselectAllObjects()
        {
            list.ForEach(items => items.IsSelected = false);
        }
    }
    extension<D>(IEnumerable<D> list)
        where D : IDeckObject
    {
        public DeckRegularDict<D> ToRegularDeckDict()
        {
            return new DeckRegularDict<D>(list);
        }
        //this was still needed for games like blades of steele.
        public DeckRegularDict<D> GetSelectedItems()
            => list.Where(items =>
           items.IsSelected == true).ToRegularDeckDict();
        public void SelectMaxOne(D thisItem)
        {
            if (thisItem.IsSelected == true)
            {
                thisItem.IsSelected = false;
                return;
            }
            list.ForEach(items => items.IsSelected = false);
            thisItem.IsSelected = true;
        }
        public void UnhighlightObjects()
        {
            list.ForEach(items =>
            {
                items.IsSelected = false;
                items.Drew = false;
            });
        }
        public void SelectAllObjects()
        {
            list.ForEach(items => items.IsSelected = true);
        }
        public bool HasSelectedObject()
        {
            return list.Any(items => items.IsSelected == true);
        }
        public bool HasUnselectedObject()
        {
            return list.Any(items => items.IsSelected == false);
        }
        public void MakeAllObjectsVisible()
        {
            list.ForEach(items => items.Visible = true);
        }
        public void MakeAllObjectsKnown()
        {
            list.ForEach(items => items.IsUnknown = false);
        }
        public int ObjectsLeft => list.Count(items => items.Visible == true);
        
    }
    extension<D>(IDeckDict<D> list)
        where D : IDeckObject
    {
        public void RemoveSelectedItems(IDeckDict<D> itemsSelected)
        {
            itemsSelected.ForEach(thisItem =>
            {
                list.RemoveObjectByDeck(thisItem.Deck); // i think.
            });
        }
        public D GetLastObjectDrawn()
        {
            return list.FindLast(items => items.Drew == true)!;
        }
        public void RemoveSelectedItems()
        {
            list.RemoveAllOnly(items => items.IsSelected == true);
        }
        public int FindIndexByDeck(int deck)
        {
            D thisCard = list.GetSpecificItem(deck);
            return list.IndexOf(thisCard);
        }
        public D GetLastObject(bool alsoRemove)
        {
            D output = list.Last();
            if (alsoRemove == true)
            {
                list.RemoveSpecificItem(output);
            }
            return output;
        }
        public D GetFirstObject(bool alsoRemove)
        {
            D output = list.First();
            if (alsoRemove == true)
            {
                list.RemoveSpecificItem(output);
            }
            return output;
        }
        public BasicList<int> GetDeckListFromObjectList()
        {
            return list.Select(Items => Items.Deck).ToBasicList();
        }
        //somehow gave casting errors when doing in blades of steels.
        public DeckRegularDict<D> GetSelectedItems()
            => list.Where(items =>
            items.IsSelected == true).ToRegularDeckDict();
    }
    extension<D>(DeckRegularDict<D> list)
        where D : IDeckObject
    {
        public void ReplaceCardPlusRemove(int oldDeck, int newDeck)
        {
            int firstIndex = list.FindIndexByDeck(oldDeck);
            int secondIndex = list.FindIndexByDeck(newDeck);
            D thisCard = list.RemoveObjectByDeck(newDeck); //that one will disappear because its going somewhere else
            if (secondIndex > firstIndex)
            {
                list[firstIndex] = thisCard;
            }
            else
            {
                list[firstIndex - 1] = thisCard;
            }
            list.ReplaceDictionary(oldDeck, newDeck, thisCard);
        }
        
    }

    extension <E>(ISimpleList<E> list)
        where E : IEnabledObject
    {
        public void SetEnabled(bool isEnabled)
        {
            list.ForEach(items => items.IsEnabled = isEnabled);
        }
    }
    extension (BasicList<int> list)
    {
        public DeckRegularDict<D> GetObjectsFromList<D>(IDeckDict<D> ListToRemove)
            where D : IDeckObject
        {
            DeckRegularDict<D> output = new();
            list.ForEach(items =>
            {
                output.Add(ListToRemove.GetSpecificItem(items));
            });
            ListToRemove.RemoveGivenList(output);
            return output;
        }
        public DeckRegularDict<D> GetNewObjectListFromDeckList<D>(
        IDeckLookUp<D> deckBase) where D : IDeckObject
        {
            DeckRegularDict<D> output = new();
            list.ForEach(items =>
            {
                output.Add(deckBase.GetSpecificItem(items));
            });
            return output;
        }
    }
    extension (string body)
    {
        public async Task<DeckRegularDict<D>> GetObjectsFromDataAsync<D>(IDeckDict<D> ListToRemove)
            where D : IDeckObject
        {
            var temps = await js1.DeserializeObjectAsync<BasicList<int>>(body);
            return temps.GetObjectsFromList(ListToRemove);
        }
    }
    extension<P>(IEnumerable<P> list)
        where P : IDeckObject, IPointsObject
    {
        public int TotalPoints => list.Sum(items => items.GetPoints);
    }
    extension <D, P>(PlayerCollection<P> players)
        where D : IDeckObject, new()
        where P : class, IPlayerSingleHand<D>, new()
    {
        public DeckRegularDict<D> CardsFromAllPlayers()
        {
            DeckRegularDict<D> output = new();
            players.ForEach(thisPlayer => output.AddRange(thisPlayer.MainHandList));
            return output;
        }
        public int WhoHasCardFromDeck(int deck)
        {
            foreach (var thisPlayer in players)
            {
                if (thisPlayer.MainHandList.ObjectExist(deck))
                {
                    return thisPlayer.Id;
                }
            }
            throw new CustomBasicException($"Nobody had deck of {deck}");
        }
    }
    extension<E>(E value)
        where E : IFastEnumSimple
    {
        public EnumSuitList GetRegularSuit()
        {
            if (value is EnumSuitList suit)
            {
                return suit;
            }
            throw new CustomBasicException("Invalid cast when getting regular suit");
        }
    }       
}