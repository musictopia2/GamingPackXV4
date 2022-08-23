namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.RummyClasses;
public class MainSetsObservable<SU, CO, RU, SE, T> : SimpleControlObservable
    where SU : IFastEnumSimple
    where CO : IFastEnumColorSimple
    where RU : IRummmyObject<SU, CO>, IDeckObject, new()
    where SE : SetInfo<SU, CO, RU, T>
{
    public bool HasFrame { get; set; } = false; // i think default should actually be false  if i am wrong; can change
    public string Text { get; set; } = "Main Sets";
    public BasicList<SE> SetList = new();
    public MainSetsObservable(CommandContainer command) : base(command) { }
    public event SetClickedEventHandler? SetClickedAsync; //we may have more than one so just do old fashioned events.
    public delegate Task SetClickedEventHandler(int setNumber, int section, int deck);
    public virtual BasicList<T> SavedSets()
    {
        BasicList<T> output = new();
        foreach (var set in SetList)
        {
            output.Add(set.SavedSet());
        }
        return output;
    }
    public void ClearBoard()
    {
        if (string.IsNullOrEmpty(Text))
        {
            throw new CustomBasicException("The text cannot be blank when starting to clear the board");
        }
        foreach (var thisSet in SetList)
        {
            thisSet.ObjectClickedAsync += ThisSet_ObjectClickedAsync;
            thisSet.SetClickedAsync += ThisSet_SetClickedAsync;
        }
        SetList.Clear();
    }
    public override void SendEnableProcesses(IBasicEnableProcess nets, Func<bool> fun)
    {
        base.SendEnableProcesses(nets, fun);
        SetList.ForEach(x => x.SendEnableProcesses(nets, fun));
    }
    public void CreateNewSet(SE thisSet)
    {
        thisSet.ObjectClickedAsync += ThisSet_ObjectClickedAsync;
        thisSet.SetClickedAsync += ThisSet_SetClickedAsync;
        thisSet.AutoSelect = EnumHandAutoType.None;
        if (_useSpecial)
        {
            thisSet.SendEnableProcesses(_networkProcess!, _customFunction!);
        }
        SetList.Add(thisSet); // we don't have scrolltoset anymore.
    }
    protected void RemoveSet(SE thisSet)
    {
        foreach (var tempSet in SetList)
        {
            if (tempSet.Equals(thisSet))
            {
                tempSet.ObjectClickedAsync -= ThisSet_ObjectClickedAsync;
                tempSet.SetClickedAsync -= ThisSet_SetClickedAsync;
                SetList.RemoveSpecificItem(thisSet);
                return;
            }
        }
        throw new CustomBasicException("Failed to remove set.  Rethink");
    }
    private async Task ThisSet_SetClickedAsync(SetInfo<SU, CO, RU, T> thisSet, int section)
    {
        if (SetClickedAsync == null)
        {
            return;
        }
        SE tempSet = (SE)thisSet;
        await SetClickedAsync.Invoke(SetList.IndexOf(tempSet) + 1, section, 0);
    }
    private async Task ThisSet_ObjectClickedAsync(SetInfo<SU, CO, RU, T> thisSet, int deck, int section)
    {
        if (SetClickedAsync == null)
        {
            return;
        }
        SE tempSet = (SE)thisSet;
        await SetClickedAsync.Invoke(SetList.IndexOf(tempSet) + 1, section, deck);
    }
    public virtual void LoadSets(BasicList<T> output)
    {
        if (SetList.Count == 0 && output.Count != 0)
        {
            throw new CustomBasicException("I think you forgot to create sets manually first.  This can't do it because of the design");
        }
        if (output.Count != SetList.Count)
        {
            throw new CustomBasicException($"Does not reconcile before loading sets  output was {output.Count} but setlist was {SetList.Count}");
        }
        int x = 0;
        SetList.ForEach(items =>
        {
            items.LoadSet(output[x]);
            x++;
        });
    }
    public void EndTurn()
    {
        SetList.ForEach(xx => xx.EndTurn());
    }
    public int HowManySets => SetList.Count;
    public SE GetIndividualSet(int index)
    {
        return SetList[index - 1];
    }
    protected override void EnableChange() { }
    protected override void PrivateEnableAlways() { }
}
