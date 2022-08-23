﻿namespace BasicGameFrameworkLibrary.SpecializedGameTypes.StockClasses;
public abstract partial class PublicPilesVM<D> : SimpleControlObservable
    where D : IDeckObject, new()
{
    public BasicList<BasicPileInfo<D>> PileList = new();
    protected override void EnableChange()
    {
        PileCommand!.ReportCanExecuteChange();
        NewCommand!.ReportCanExecuteChange();
    }
    protected abstract int MaximumAllowed { get; }
    protected override void PrivateEnableAlways() { }
    public int NextNumberNeeded(int index)
    {
        var thisPile = PileList[index];
        if (thisPile.ObjectList.Count == 0)
        {
            return 1;
        }
        if (thisPile.ObjectList.Count > MaximumAllowed)
        {
            throw new CustomBasicException("Should have cleared the piles because the numbers goes up to 15");
        }
        return thisPile.ObjectList.Count + 1;
    }
    public void CreateNewPile(D thisCard) // because this appears to be the only game that has it this way. (?)
    {
        BasicPileInfo<D> thisPile = new();
        thisPile.ObjectList.Add(thisCard);
        PileList.Add(thisPile); // the bindings should do what it needs to (because of observable) well see
    }
    public void UnselectAllPiles()
    {
        foreach (var thisPile in PileList)
        {
            thisPile.IsSelected = false;// i think its this simple (?)
        }
    }
    public void ClearBoard()
    {
        PileList.Clear(); // i think its as simple as clearing the pilelist (?)
    }
    public bool NeedToRemovePile(int pile)
    {
        if (PileList[pile].ObjectList.Count == MaximumAllowed)
        {
            return true;
        }
        if (PileList[pile].ObjectList.Count > MaximumAllowed)
        {
            throw new Exception("Should have already cleared the pile");
        }
        return false;
    }
    public DeckRegularDict<D> EmptyPileList(int pile)
    {
        var thisPile = PileList[pile];
        if (thisPile.ObjectList.Count != MaximumAllowed)
        {
            throw new CustomBasicException($"Must have {MaximumAllowed}  cards to empty a pile; not {thisPile.ObjectList.Count}");
        }
        var output = thisPile.ObjectList.ToRegularDeckDict();
        PileList.RemoveSpecificItem(thisPile);
        return output;
    }
    public void AddCardToPile(int pile, D thisCard)
    {
        var thisPile = PileList[pile];
        thisPile.ObjectList.Add(thisCard); // i think its this simple
    }
    public int MaxPiles()
    {
        return PileList.Count;
    }
    public event PileClickedEventHandler? PileClickedAsync;
    public delegate Task PileClickedEventHandler(int Index);
    public event NewPileClickedEventHandler? NewPileClickedAsync;
    public delegate Task NewPileClickedEventHandler();
    public ControlCommand? NewCommand { get; set; }
    public ControlCommand? PileCommand { get; set; }
    [Command(EnumCommandCategory.Control, Name = nameof(NewCommand))]
    private async Task PrivateNewAsync()
    {
        if (NewPileClickedAsync == null)
        {
            return;
        }
        await NewPileClickedAsync.Invoke();
    }
    [Command(EnumCommandCategory.Control, Name = nameof(PileCommand))]
    private async Task PrivateClickAsync(BasicPileInfo<D> pile)
    {
        if (PileClickedAsync == null)
        {
            return;
        }
        await PileClickedAsync.Invoke(PileList.IndexOf(pile));
    }
    public PublicPilesVM(CommandContainer command) : base(command)
    {
        CreateCommands();
    }
    partial void CreateCommands();
    protected bool HasCard(int pile)
    {
        if (PileList[pile - 1].ObjectList.Count == 0)
        {
            return false;
        }
        return true;
    }
    protected D GetLastCard(int pile)
    {
        return PileList[pile - 1].ObjectList.Last(); // i think
    }
}
