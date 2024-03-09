namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public class CommandContainer
{
    private readonly BasicList<IGameCommand> _commandList = new();
    private readonly BasicList<IGameCommand> _openList = new();
    public Action? ExecutingChanged { get; set; }
    private readonly BasicList<IControlObservable> _controlList = new();
    private readonly Dictionary<string, Action> _specialActions = new();
    public Action? ParentAction { get; set; }
    public event Action? CustomStateHasChanged;
    public void ResetCustomStates()
    {
        CustomStateHasChanged = null; //to avoid memory leaks (just in case i forget to unsubscribe.
        //this is needed to avoid memory leaks.
    }
    public CommandContainer()
    {
        IsExecuting = true;
    }
    public void UpdateAll()
    {
        if (CustomStateHasChanged is null)
        {
            ParentAction?.Invoke();
            return;
        }
        CustomStateHasChanged.Invoke(); //this means if there are several then will call those as well.
    }
    public void UpdateSpecificAction(string key)
    {
        if (_specialActions.ContainsKey(key) == false)
        {
            return;
        }
        _specialActions[key].Invoke();
    }
    public bool CanExecuteManually()
    {
        return !IsExecuting;
    }
    public void StartExecuting()
    {
        if (IsExecuting)
        {
            Processing = true;
            return;
        }
        IsExecuting = true;
        Processing = true;
    }
    public async Task ProcessCustomCommandAsync<T>(Func<T, Task> action, T argument)
    {
        StartExecuting();
        await action.Invoke(argument);
        StopExecuting();
    }
    public async Task ProcessCustomCommandAsync(Func<Task> action)
    {
        StartExecuting();
        await action.Invoke();
        StopExecuting();
    }
    public void StopExecuting()
    {
        if (ManuelFinish == false)
        {
            IsExecuting = false;
        }
        Processing = false;
    }
    public void ClearLists()
    {
        _commandList.Clear();
        _openList.Clear();
        _controlList.Clear();
    }
    private bool _executing;
    /// <summary>
    /// This is used when its not even your turn.
    /// Use Processing if you are able to do things out of turn as long
    /// as the other variable is false.
    /// </summary>
    /// <remarks>This is used when its not even your turn.
    /// Use Processing if you are able to do things out of turn as long
    /// as the other variable is false.</remarks>
    public bool IsExecuting
    {
        get
        {
            return _executing; //eventually have to change once i have multiplayer processes completed.
        }
        set
        {
            if (value == _executing)
            {
                return;
            }
            _executing = value;
            ReportAll();
        }
    }
    private bool _openBusy;
    public bool OpenBusy
    {
        get
        {
            return _openBusy;
        }
        set
        {
            if (value == _openBusy)
            {
                return;
            }
            _openBusy = value;
            ReportOpen();
        }
    }
    public void ReportOpen()
    {
        _openList.ForEach(x => x.ReportCanExecuteChange());
        UpdateAll();
    }
    /// <summary>
    /// the purpose of this one is if a move is in progress, then even if being rejoined, then has to wait until move is not in progress anymore.
    /// </summary>

    private bool _processing = true;
    public bool Processing
    {
        get { return _processing; }
        set
        {
            if (value == _processing)
            {
                return;
            }
            _processing = value;
            ReportLimited();
        }
    }
    public void ReportLimited()
    {
        ReportItems(EnumCommandBusyCategory.Limited);
        UpdateAll();
    }
    public bool ManuelFinish { get; set; } = false;
    public void ManualReport()
    {
        _commandList.ForEach(x => x.ReportCanExecuteChange());
        _controlList.ForEach(x => x.ReportCanExecuteChange());
    }
    public void RemoveOldItems(object payLoad)
    {
        _commandList.RemoveAllOnly(x => x.Context == payLoad);
    }
    public void ReportAll()
    {
        ReportItems(EnumCommandBusyCategory.None);
        ExecutingChanged?.Invoke(); //i don't think there is a need for more than one process to subscribe.  since its possible to have many timimg issues for maui.
        UpdateAll();
    }
    private void ReportItems(EnumCommandBusyCategory thisBusy)
    {
        _commandList.ForConditionalItems(items => items.BusyCategory == thisBusy
        , items => items.ReportCanExecuteChange());
        _controlList.ForConditionalItems(items => items.BusyCategory == thisBusy
        , items => items.ReportCanExecuteChange());
    }
    public void AddOpen(IGameCommand thisOpen)
    {
        _openList.Add(thisOpen);
    }
    public void AddCommand(IGameCommand command)
    {
        _commandList.Add(command);
    }
    public void AddControl(IControlObservable thisControl)
    {
        _controlList.Add(thisControl);
    }
    public void AddAction(Action action, string key)
    {
        if (_specialActions.ContainsKey(key) == false)
        {
            _specialActions.Add(key, action);
        }
    }
    public void RemoveCommand(IGameCommand command)
    {
        _commandList.RemoveSpecificItem(command);
    }
    public void RemoveAction(string key)
    {
        if (_specialActions.ContainsKey(key) == false)
        {
            return;
        }
        _specialActions.Remove(key);
    }
}