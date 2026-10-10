namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public class StateList<T> : BasicList<T>, IStateChanged
{
    public StateList(IEnumerable<T> thisList) : base(thisList) { }
    public StateList() { }
    public event Action? StateChanged;
    protected override void ListChanged()
    {
        StateChanged?.Invoke();
    }
}