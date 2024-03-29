﻿namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.RummyClasses;
public class TempSetsObservable<S, C, R>(CommandContainer command, IGamePackageResolver resolver)
    where S : IFastEnumSimple
    where C : IFastEnumColorSimple
    where R : IDeckObject, IRummmyObject<S, C>, new()
{
    public Func<int, Task>? SetClickedAsync { get; set; }
    public int Spacing { get; set; }
    public int HowManySets { get; set; } = 5;
    public BasicList<RummyHandObservable<S, C, R>> SetList = new();
    public DeckRegularDict<R> ObjectList(int index)
    {
        return SetList[index - 1].HandList;
    }

    private IEventAggregator? _thisE;

    public void ReportCanExecuteChange()
    {
        SetList.ForEach(x => x.ReportCanExecuteChange());
    }
    public void Init(IEnableAlways enables)
    {
        if (SetList.Count > 0)
        {
            return;
        }
        _thisE = resolver.Resolve<IEventAggregator>();
        int x;
        var loopTo = HowManySets;
        RummyHandObservable<S, C, R> thisSet;
        for (x = 1; x <= loopTo; x++)
        {
            thisSet = new(command, resolver);
            thisSet.AutoSelect = EnumHandAutoType.None;
            thisSet.SendAlwaysEnable(enables);
            thisSet.Text = "Set";
            thisSet.SetClickedAsync = ThisSet_SetClickedAsync;
            SetList.Add(thisSet);
        }
    }
    private async Task ThisSet_SetClickedAsync(RummyHandObservable<S, C, R> thisSet)
    {
        if (SetClickedAsync == null)
        {
            return;
        }
        if (thisSet.DidClickObject == true)
        {
            thisSet.DidClickObject = false;
            return;
        }
        await SetClickedAsync.Invoke(SetList.IndexOf(thisSet) + 1);
    }
    public void ResetCards()
    {
        SetList.ForEach(Items => Items.DidClickObject = false);
    }
    public DeckRegularDict<R> ListAllObjects()
    {
        DeckRegularDict<R> output = new();
        SetList.ForEach(thisTemp =>
        {
            output.AddRange(thisTemp.HandList);
        });
        return output;
    }
    public void UnselectAllCards()
    {
        SetList.ForEach(items => items.UnselectAllObjects());
    }
    public void EndTurn()
    {
        SetList.ForEach(items => items.EndTurn());
    }
    public void ClearBoard()
    {
        SetList.ForEach(items => items.ClearHand());
        PublicCount();
    }
    public void ClearBoard(int index)
    {
        SetList[index - 1].ClearHand();
        PublicCount();
    }
    public void PublicCount()
    {
        UpdateCountEventModel thisU = new();
        if (_thisE == null)
        {
            return;
        }
        thisU.ObjectCount = TotalObjects;
        if (_thisE.HandlerRegularExistsFor<UpdateCountEventModel>() == false)
        {
            return;
        }
        _thisE!.Publish(thisU);
    }
    public int TotalObjects => SetList.Sum(xx => xx.HandList.Count);
    public int HowManySelectedObjects => SetList.Sum(xx => xx.HowManySelectedObjects);
    public bool HasSelectedObject => SetList.Any(xx => xx.HowManySelectedObjects > 0);
    public int PileForSelectedObject
    {
        get
        {
            RummyHandObservable<S, C, R> thisVM = SetList.FirstOrDefault(xx => xx.HowManySelectedObjects > 0)! ?? throw new CustomBasicException("There was no pile with only one selected card.  Find out what happened");
            return SetList.IndexOf(thisVM) + 1;
        }
    }
    public int DeckForSelectedObjected(int pile)
    {
        return SetList[pile - 1].ObjectSelected();
    }
    public DeckRegularDict<R> ListObjectsRemoved()
    {
        var output = ListAllObjects();
        ClearBoard();
        return output;
    }
    public bool HasObject(int deck)
    {
        var thisList = ListAllObjects();
        return thisList.ObjectExist(deck);
    }
    public void RemoveObject(int deck)
    {
        foreach (var thisSet in SetList)
        {
            if (thisSet.HandList.ObjectExist(deck))
            {
                thisSet.HandList.RemoveObjectByDeck(deck);
                PublicCount();
                return;
            }
        }
        throw new CustomBasicException($"There is no card with the deck {deck} to remove for tempsets");
    }
    public DeckRegularDict<R> ListSelectedObjects(bool alsoRemove = false)
    {
        DeckRegularDict<R> output = new();
        SetList.ForEach(thisTemp =>
        {
            output.AddRange(thisTemp.ListSelectedObjects(alsoRemove));
        });
        if (alsoRemove == true)
        {
            PublicCount();
        }
        return output;
    }
    public DeckRegularDict<R> SelectedObjectsRemoved()
    {
        return ListSelectedObjects(true);
    }
    public R GetSelectedObject => ListSelectedObjects().Single();
    public void AddCards(int whichOne, IDeckDict<R> cardList)
    {
        var tempList = SelectedObjectsRemoved();
        cardList.AddRange(tempList);
        if (cardList.Count == 0)
        {
            return;
        }
        SetList[whichOne - 1].AddCards(cardList);
        PublicCount();
    }
}