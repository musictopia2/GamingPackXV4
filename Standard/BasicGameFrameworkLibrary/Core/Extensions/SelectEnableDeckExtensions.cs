namespace BasicGameFrameworkLibrary.Core.Extensions;
public static class SelectEnableDeckExtensions
{
    public static string GetColorFromEnum<E>(this BasicPickerData<E> piece) where E : struct, IFastEnumColorSimple
    {
        return piece.EnumValue.Color; //now can do this way.
    }
    public static void SelectUnselectItem(this ISelectableObject thisItem)
    {
        thisItem.IsSelected = !thisItem.IsSelected;
    }
    public static void SelectUnselectItem<S>(this ISimpleList<S> thisList, int index) //has to be generics or casting problems.
        where S : ISelectableObject
    {
        int i = 0;
        foreach (var item in thisList)
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
    public static void SelectSpecificItem<S, V>(this ISimpleList<S> thisList, Func<S, V> selector, V value)
        where S : ISelectableObject
    {
        thisList.ForEach(items =>
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
    public static void SelectSeveralItems<S, V>(this ISimpleList<S> thisList, Func<S, V> selector,
        ISimpleList<V> valuelist) where S : ISelectableObject
    {
        thisList.UnselectAllObjects(); //has to first select all objects.

        valuelist.ForEach(value =>
        {
            thisList.ForEach(items =>
            {
                if (selector(items)!.Equals(value))
                {
                    items.IsSelected = true;
                }
            });
        });
    }
    public static BasicList<S> GetSelectedItems<S>(this ISimpleList<S> thisList)
        where S : ISelectableObject => thisList.Where(items =>
        items.IsSelected == true).ToBasicList();

    //this was still needed for games like blades of steele.
    public static DeckRegularDict<D> GetSelectedItems<D>(this IDeckDict<D> thisList)
       where D : IDeckObject => thisList.Where(items =>
       items.IsSelected == true).ToRegularDeckDict();

    public static int HowManySelectedItems<S>(this ISimpleList<S> thisList) where S : ISelectableObject
    {
        return thisList.Count(items => items.IsSelected);
    }

    public static void UnselectAllObjects<S>(this ISimpleList<S> thisList) where S : ISelectableObject
    {
        thisList.ForEach(items => items.IsSelected = false);
    }

    public static void SetEnabled<E>(this ISimpleList<E> thisList, bool isEnabled) where E : IEnabledObject
    {
        thisList.ForEach(items => items.IsEnabled = isEnabled);
    }
    public static async Task<BasicList<int>> GetSavedIntegerListAsync(this string data)
    {
        return await js.DeserializeObjectAsync<BasicList<int>>(data);
    }

    public static DeckRegularDict<D> ToRegularDeckDict<D>(this IEnumerable<D> thisList) where D : IDeckObject
    {
        return new DeckRegularDict<D>(thisList);
    }
    public static void SelectMaxOne<D>(this IDeckDict<D> thisList, D thisItem) where D : IDeckObject
    {
        if (thisItem.IsSelected == true)
        {
            thisItem.IsSelected = false;
            return;
        }
        thisList.ForEach(items => items.IsSelected = false);
        thisItem.IsSelected = true;
    }
    public static void UnhighlightObjects<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        thisList.ForEach(items =>
        {
            items.IsSelected = false;
            items.Drew = false;
        });
    }
    public static void RemoveSelectedItems<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        thisList.RemoveAllOnly(items => items.IsSelected == true);
    }
    public static void RemoveSelectedItems<D>(this IDeckDict<D> thisList, IDeckDict<D> itemsSelected) where D : IDeckObject
    {
        itemsSelected.ForEach(thisItem =>
        {
            thisList.RemoveObjectByDeck(thisItem.Deck); // i think.
        });
    }
    public static void SelectAllObjects<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        thisList.ForEach(items => items.IsSelected = true);
    }

    public static bool HasSelectedObject<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        return thisList.Any(items => items.IsSelected == true);
    }
    public static bool HasUnselectedObject<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        return thisList.Any(items => items.IsSelected == false);
    }
    public static void MakeAllObjectsVisible<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        thisList.ForEach(items => items.Visible = true);
    }
    public static void MakeAllObjectsKnown<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        thisList.ForEach(items => items.IsUnknown = false);
    }
    public static D GetLastObjectDrawn<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        return thisList.FindLast(items => items.Drew == true)!;
    }
    public static DeckRegularDict<D> GetObjectsFromList<D>(this BasicList<int> thisList
        , IDeckDict<D> ListToRemove) where D : IDeckObject
    {
        DeckRegularDict<D> output = new();
        thisList.ForEach(items =>
        {
            output.Add(ListToRemove.GetSpecificItem(items));
        });
        ListToRemove.RemoveGivenList(output);
        return output;
    }
    public static async Task<DeckRegularDict<D>> GetObjectsFromDataAsync<D>(this string body
        , IDeckDict<D> ListToRemove) where D : IDeckObject
    {
        var temps = await js.DeserializeObjectAsync<BasicList<int>>(body);
        return temps.GetObjectsFromList(ListToRemove);
    }
    public static int ObjectsLeft(this IDeckDict<IDeckObject> thisList)
    {
        return thisList.Count(items => items.Visible == true);
    }
    public static int FindIndexByDeck<D>(this IDeckDict<D> thisList, int deck) where D : IDeckObject
    {
        D thisCard = thisList.GetSpecificItem(deck);
        return thisList.IndexOf(thisCard);
    }
    public static void ReplaceCardPlusRemove<D>(this DeckRegularDict<D> thisList, int oldDeck, int newDeck) where D : IDeckObject
    {
        int firstIndex = thisList.FindIndexByDeck(oldDeck);
        int secondIndex = thisList.FindIndexByDeck(newDeck);
        D thisCard = thisList.RemoveObjectByDeck(newDeck); //that one will disappear because its going somewhere else
        if (secondIndex > firstIndex)
        {
            thisList[firstIndex] = thisCard;
        }
        else
        {
            thisList[firstIndex - 1] = thisCard;
        }
        thisList.ReplaceDictionary(oldDeck, newDeck, thisCard);
    }
    public static D GetLastObject<D>(this IDeckDict<D> thisList, bool alsoRemove) where D : IDeckObject
    {
        D output = thisList.Last();
        if (alsoRemove == true)
        {
            thisList.RemoveSpecificItem(output);
        }
        return output;
    }
    public static D GetFirstObject<D>(this IDeckDict<D> thisList, bool alsoRemove) where D : IDeckObject
    {
        D output = thisList.First();
        if (alsoRemove == true)
        {
            thisList.RemoveSpecificItem(output);
        }
        return output;
    }
    public static BasicList<int> GetDeckListFromObjectList<D>(this IDeckDict<D> thisList) where D : IDeckObject
    {
        return thisList.Select(Items => Items.Deck).ToBasicList();
    }
    public static DeckRegularDict<D> GetNewObjectListFromDeckList<D>(this BasicList<int> thisList,
        IDeckLookUp<D> deckBase) where D : IDeckObject
    {
        DeckRegularDict<D> output = new();
        thisList.ForEach(items =>
        {
            output.Add(deckBase.GetSpecificItem(items));
        });
        return output;
    }
    public static async Task<DeckRegularDict<D>> GetNewObjectListFromDeckListAsync<D>(this string data,
        IDeckLookUp<D> thisBase) where D : IDeckObject
    {
        DeckRegularDict<D> output = new();

        BasicList<int> thisList = await js.DeserializeObjectAsync<BasicList<int>>(data);
        thisList.ForEach(items =>
        {
            output.Add(thisBase.GetSpecificItem(items));
        });
        return output;
    }
    public static int TotalPoints<P>(this IDeckDict<P> thisList)
        where P : IDeckObject, IPointsObject
    {
        return thisList.Sum(items => items.GetPoints);
    }
    public static DeckRegularDict<D> CardsFromAllPlayers<D, P>(this PlayerCollection<P> playerList)
        where D : IDeckObject, new()
        where P : class, IPlayerSingleHand<D>, new()
    {
        DeckRegularDict<D> output = new();
        playerList.ForEach(thisPlayer => output.AddRange(thisPlayer.MainHandList));
        return output;
    }
    public static int WhoHasCardFromDeck<D, P>(this PlayerCollection<P> playerList, int deck)
        where D : IDeckObject, new()
        where P : class, IPlayerSingleHand<D>, new()
    {
        foreach (var thisPlayer in playerList)
        {
            if (thisPlayer.MainHandList.ObjectExist(deck))
            {
                return thisPlayer.Id;
            }
        }
        throw new CustomBasicException($"Nobody had deck of {deck}");
    }
    public static EnumSuitList GetRegularSuit<E>(this E value)
        where E : IFastEnumSimple
    {
        if (value is EnumSuitList suit)
        {
            return suit;
        }
        throw new CustomBasicException("Invalid cast when getting regular suit");
        //return (EnumSuitList)Enum.Parse(typeof(EnumSuitList), value.ToString());
    }
}