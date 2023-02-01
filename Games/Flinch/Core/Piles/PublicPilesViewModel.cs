namespace Flinch.Core.Piles;
public partial class PublicPilesViewModel : SimpleControlObservable, ISerializable
{
    public BasicList<BasicPileInfo<FlinchCardInformation>> PileList = new();
    public int NextNumberNeeded(int index)
    {
        var thisPile = PileList[index];
        if (thisPile.ObjectList.Count == 0)
        {
            return 1;
        }
        if (thisPile.ObjectList.Count > 15)
        {
            throw new CustomBasicException("Should have cleared the piles because the numbers goes up to 15");
        }
        return thisPile.ObjectList.Count + 1;
    }
    public void CreateNewPile(FlinchCardInformation thisCard) // because this appears to be the only game that has it this way. (?)
    {
        BasicPileInfo<FlinchCardInformation> thisPile = new();
        thisPile.ObjectList.Add(thisCard);
        PileList.Add(thisPile);
    }
    public void UnselectAllPiles()
    {
        foreach (var thisPile in PileList)
        {
            thisPile.IsSelected = false;
        }
    }
    public async Task<string> GetSavedPilesAsync()
    {
        return await js1.SerializeObjectAsync(PileList);
    }
    public async Task LoadSavedPilesAsync(string thisStr)
    {
        PileList = await js1.DeserializeObjectAsync<BasicList<BasicPileInfo<FlinchCardInformation>>>(thisStr);
    }
    public void ClearBoard()
    {
        PileList.Clear();
    }
    public bool NeedToRemovePile(int pile)
    {
        if (PileList[pile].ObjectList.Count == 15)
        {
            return true;
        }
        if (PileList[pile].ObjectList.Count > 15)
        {
            throw new CustomBasicException("Should have already cleared the pile");
        }
        return false;
    }
    public DeckRegularDict<FlinchCardInformation> EmptyPileList(int whichOne)
    {
        var thisPile = PileList[whichOne];
        if (thisPile.ObjectList.Count != 15)
        {
            throw new CustomBasicException($"Must have 15 cards to empty a pile; not {thisPile.ObjectList.Count}");
        }
        var tempList = thisPile.ObjectList.ToRegularDeckDict();
        PileList.RemoveSpecificItem(thisPile);
        if (tempList.Count != 15)
        {
            throw new CustomBasicException($"Must have 15 cards in the list, not {tempList.Count}");
        }
        return tempList;
    }
    public void AddCardToPile(int pile, FlinchCardInformation thisCard)
    {
        var thisPile = PileList[pile];
        thisPile.ObjectList.Add(thisCard);
    }
    public int MaxPiles()
    {
        return PileList.Count;
    }
    public Func<int, Task>? PileClickedAsync { get; set; }
    public ControlCommand? PileCommand { get; set; }
    [Command(EnumCommandCategory.Control)]
    private async Task PrivatePileAsync(BasicPileInfo<FlinchCardInformation> card)
    {
        if (PileClickedAsync == null)
        {
            return;
        }
        await PileClickedAsync.Invoke(PileList.IndexOf(card));
    }

    public PublicPilesViewModel(CommandContainer command) : base(command)
    {
        CreateCommands();
    }
    partial void CreateCommands();
    protected override void EnableChange()
    {
        PileCommand!.ReportCanExecuteChange();
    }
    protected override void PrivateEnableAlways() { }
}
