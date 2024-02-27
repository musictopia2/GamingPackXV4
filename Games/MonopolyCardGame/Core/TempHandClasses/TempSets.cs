namespace MonopolyCardGame.Core.TempHandClasses;
public class TempSets(CommandContainer command, IGamePackageResolver resolver, MonopolyCardGameGameContainer container)
{
    public Func<int, Task>? SetClickedAsync { get; set; }
    public int Spacing { get; set; }
    public int HowManySets { get; set; } = 5; // defaults at 5
    public BasicList<TempHand> SetList = []; //has to be public because of data binding
    public DeckRegularDict<MonopolyCardGameCardInformation> ObjectList(int index)
    {
        return SetList[index - 1].HandList; //i send in one based.
    }
    private IEventAggregator? _thisE;
    public void ReportCanExecuteChange()
    {
        SetList.ForEach(x => x.ReportCanExecuteChange());
    }
    public BasicList<OrganizeModel> SaveTempSets()
    {
        BasicList<OrganizeModel> output = [];
        int x = 0;
        foreach (var item in SetList)
        {
            x++;
            OrganizeModel model = new();
            model.SetNumber = x;
            model.Cards = item.HandList.GetDeckListFromObjectList();
            output.Add(model);
        }
        return output;
    }
    private void PossibleAutoResume()
    {
        if (container.TempSets.Count > 0)
        {
            var player = container.PlayerList!.GetSelf();
            bool hadAny = false;
            foreach (var item in container.TempSets)
            {
                var current = SetList[item.SetNumber - 1];
                var cards = item.Cards.GetNewObjectListFromDeckList(container.DeckList);
                DeckRegularDict<MonopolyCardGameCardInformation> toAdd = [];
                foreach (var card in cards)
                {
                    if (player.MainHandList.ObjectExist(card.Deck))
                    {

                        player.MainHandList.RemoveObjectByDeck(card.Deck);
                        player.AdditionalCards.Add(card); //if i remove from hand, must add to additional cards so sends to other players properly.
                        hadAny = true;
                        toAdd.Add(card);
                    }
                }
                current.AddCards(toAdd);
            }
            if (hadAny)
            {
                PublicCount();
            }
        }
    }
    public void Init(IEnableAlways enables)
    {
        if (SetList.Count > 0)
        {
            PossibleAutoResume();
            return;
        }
        _thisE = resolver.Resolve<IEventAggregator>();
        int x;
        var loopTo = HowManySets;
        TempHand thisSet;
        for (x = 1; x <= loopTo; x++)
        {
            thisSet = new(command, resolver);
            thisSet.AutoSelect = EnumHandAutoType.None;
            thisSet.SendAlwaysEnable(enables);
            thisSet.Text = "Set";
            thisSet.SetClickedAsync = ThisSet_SetClickedAsync;
            SetList.Add(thisSet);
        }
        PossibleAutoResume();
    }
    private async Task ThisSet_SetClickedAsync(TempHand thisSet)
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
        await SetClickedAsync.Invoke(SetList.IndexOf(thisSet) + 1); //wanted to make it one based.
    }
    public void ResetCards()
    {
        SetList.ForEach(Items => Items.DidClickObject = false);
    }
    public DeckRegularDict<MonopolyCardGameCardInformation> ListAllObjects()
    {
        DeckRegularDict<MonopolyCardGameCardInformation> output = [];
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
        SetList[index - 1].ClearHand(); //sending in one based.
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
    public bool HasAnyInSet(int x) => SetList[x - 1].HandList.Count > 0;
    public MonopolyCardGameCardInformation GetFirstCardInSet(int x)
    {
        var set = SetList[x - 1];
        return set.HandList.First();
    }
    public int TotalObjects => SetList.Sum(x => x.HandList.Count);
    public int HowManySelectedObjects => SetList.Sum(x => x.HowManySelectedObjects);
    public int HowManyUnselectedObjects => SetList.Sum(x => x.HowManyUnselectedObjects);
    public bool HasSelectedObject => SetList.Any(x => x.HowManySelectedObjects > 0);
    public int PileForSelectedObject
    {
        get
        {
            TempHand thisVM = SetList.FirstOrDefault(Items => Items.HowManySelectedObjects > 0)! ?? throw new CustomBasicException("There was no pile with only one selected card.  Find out what happened");
            return SetList.IndexOf(thisVM) + 1; //returning 1 based.
        }
    }
    public int DeckForSelectedObjected(int pile)
    {
        return SetList[pile - 1].ObjectSelected();
    }
    public DeckRegularDict<MonopolyCardGameCardInformation> ListObjectsRemoved()
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
    public DeckRegularDict<MonopolyCardGameCardInformation> ListSelectedObjects(bool alsoRemove = false)
    {
        DeckRegularDict<MonopolyCardGameCardInformation> output = [];
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
    public DeckRegularDict<MonopolyCardGameCardInformation> SelectedObjectsRemoved()
    {
        return ListSelectedObjects(true);
    }
    public MonopolyCardGameCardInformation GetSelectedObject => ListSelectedObjects().Single();
    public void AddCards(int whichOne, IDeckDict<MonopolyCardGameCardInformation> cardList)
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
