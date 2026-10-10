namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public class CommandContainer
{
    //start out with no open list.
    private readonly BasicList<IGameCommand> _commands = [];
    private readonly BasicList<IControlObservable> _controls = new();
    private readonly BasicList<IGameCommand> _opens = new();

    //public static void UpdateScreen(string key)
    //{
    //    CommandContainer container = Resolver!.Resolve<CommandContainer>();
    //    container.UpdateSpecificAction(key);
    //}

    private readonly HashSet<string> _pendingRefreshes = [];
    public void QueueRefreshAfterCommand(string key)
    {
        _pendingRefreshes.Add(key);
    }
    private void RefreshPendingActions()
    {
        if (_pendingRefreshes.Count == 0)
        {
            return;
        }

        var keys = _pendingRefreshes.ToArray();
        _pendingRefreshes.Clear();

        foreach (var key in keys)
        {
            UpdateSpecificAction(key);
        }
    }

    private readonly Dictionary<string, Action> _specificActions = [];

    public void AddSpecificAction(string key, Action action)
    {
        _specificActions.TryAdd(key, action);
    }

    public void RemoveSpecificAction(string key)
    {
        _specificActions.Remove(key);
    }

    public void UpdateSpecificAction(string key)
    {
        if (_specificActions.TryGetValue(key, out Action? action))
        {
            action.Invoke();
        }
    }

    private bool _openBusy;
    public bool OpenBusy
    {
        get => _openBusy;
        set
        {
            if (_openBusy == value)
            {
                return;
            }

            _openBusy = value;
            ReportOpen();
        }
    }

    private void ReportOpen() //may not need this public (not sure yet)
    {
        _opens.ForEach(x => x.ReportCanExecuteChange());
    }

    //public event Action? StateChanged;
    public Action? ExecutingChanged { get; set; }
    public bool CanExecuteManually()
    {
        return IsExecuting == false;
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
    public void StopExecuting()
    {
        if (ManuelFinish == false)
        {
            IsExecuting = false;
        }
        Processing = false;
        RefreshPendingActions();
    }
    public async Task ProcessCustomCommandAsync<T>(
    Func<T, Task> action,
    T argument)
    {
        StartExecuting();

        try
        {
            await action(argument);
        }
        finally
        {
            StopExecuting();
        }
    }

    //risk no updateallcontrols.  if needed again, rethink.

    //public void UpdateAllControls()
    //{
    //    foreach (var control in _controls)
    //    {
    //        control.ReportCanExecuteChange();
    //        //await Task.Yield();
    //    }
    //}
    public async Task ProcessCustomCommandAsync(Func<Task> action)
    {
        StartExecuting();

        try
        {
            await action();
        }
        finally
        {
            StopExecuting();
        }
    }
    public void ClearLists()
    {
        _commands.Clear();
        _controls.Clear();
        _opens.Clear();
        _specificActions.Clear();
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
            ReportNormalExecutionChanged();
        }
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
            ReportLimitedExecutionChanged();
            //ReportLimited();
        }
    }
    public void ReportLimitedExecutionChanged()
    {
        ReportItems(EnumCommandBusyCategory.Limited);
    }
    public void ReportNormalExecutionChanged()
    {
        ReportItems(EnumCommandBusyCategory.None);
        ExecutingChanged?.Invoke();
    }
    public void ReportAllCommandStates()
    {
        _commands.ForEach(x => x.ReportCanExecuteChange());
        _controls.ForEach(x => x.ReportCanExecuteChange());
    }

    public bool ManuelFinish { get; set; } = false;
    public void AddCommand(IGameCommand command)
    {
        _commands.Add(command);
    }
    public void RemoveCommand(IGameCommand command)
    {
        _commands.RemoveSpecificItem(command);
    }


    private void ReportItems(EnumCommandBusyCategory thisBusy)
    {
        _commands.ForConditionalItems(
            x => x.BusyCategory == thisBusy,
            x => x.ReportCanExecuteChange());

        _controls.ForConditionalItems(
            x => x.BusyCategory == thisBusy,
            x => x.ReportCanExecuteChange());
    }


    //public void ReportCommandStateChanged()
    //{
    //    _controls.ForEach(x => x.ReportCanExecuteChange());
    //    //start with no stage changed here (requires rethinking for now).



    //    //StateChanged?.Invoke();
    //}
    public void AddControl(IControlObservable thisControl)
    {
        _controls.Add(thisControl);
    }
}