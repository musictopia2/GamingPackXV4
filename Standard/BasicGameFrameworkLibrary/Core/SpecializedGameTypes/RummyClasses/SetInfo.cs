namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.RummyClasses;
public abstract class SetInfo<S, C, R, T>(CommandContainer command) : HandObservable<R>(command)
    where S : IFastEnumSimple
    where C : IFastEnumColorSimple
    where R : IDeckObject, IRummmyObject<S, C>, new()
{
    public abstract T SavedSet();
    public abstract void LoadSet(T save);
    private int _tempSection;
    public Func<SetInfo<S, C, R, T>, int, Task>? SetClickedAsync { get; set; }
    public new Func<SetInfo<S, C, R, T>, int, int, Task>? ObjectClickedAsync { get; set; }
    protected bool CanExpandRuns;
    protected virtual bool IsRun()
    {
        return CanExpandRuns;
    }
    public override bool HasSections => true;
    protected virtual bool CanClickMainBoard()
    {
        return true;
    }
    public virtual bool CanClickSingleObject()
    {
        if (IsRun() == false)
        {
            return true;
        }
        if (_tempSection == 0)
        {
            return true;
        }
        if (_tempSection == HandList.Count - 1)
        {
            return true;
        }
        return false;
    }
    protected override async Task ProcessObjectClickedAsync(R thisObject, int index)
    {
        if (ObjectClickedAsync == null)
        {
            return;
        }
        _tempSection = index;
        if (CanClickSingleObject() == false)
        {
            return;
        }
        int thisSection;
        if (IsRun() == true)
        {
            if (index == 0)
            {
                thisSection = 1;
            }
            else
            {
                thisSection = 2;
            }
        }
        else
        {
            thisSection = 1;
        }
        if (thisSection == 0)
        {
            throw new CustomBasicException("Section cannot be 0 for card clicked");
        }
        SectionClicked = 0; // reset back to 0
        await ObjectClickedAsync.Invoke(this, thisObject.Deck, thisSection);
    }
    protected override async Task PrivateBoardSingleClickedAsync()
    {
        if (SetClickedAsync == null)
        {
            return;
        }
        if (IsRun() == true)
        {
            return;
        }
        if (CanClickMainBoard() == false)
        {
            return;
        }
        int thisSection;
        thisSection = SectionClicked;
        if (CanExpandRuns == true)
        {
            thisSection = 1;
        }
        SectionClicked = 0;
        await SetClickedAsync.Invoke(this, thisSection);
    }
}