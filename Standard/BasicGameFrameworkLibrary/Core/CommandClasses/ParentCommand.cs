namespace BasicGameFrameworkLibrary.Core.CommandClasses;

public abstract class ParentCommand
{
    public event EventHandler CanExecuteChanged = delegate { };
    protected readonly object _model;
    protected readonly Func<Task>? _simpleAsync1;
    protected readonly Func<object?, Task>? _simpleAsync2;
    protected readonly Action? _simpleAction1;
    protected readonly Action<object?>? _simpleAction2;
    protected readonly Func<bool>? _canExecute1;
    protected readonly Func<object?, bool>? _canExecute2;
    protected readonly string _functionName;
    protected bool _isExecuting;
    protected bool _hasMethodParameters;
    protected bool _hasCanParameters;
    protected bool _isAsync;
    public object Context => _model;
    protected virtual void AddCommand() { }
    public Action? UpdateBlazor { get; set; }
    public ParentCommand(object model,
        Func<Task>? simpleAsync1 = null, Func<object?, Task>? simpleAsync2 = null,
        Action? simpleAction1 = null, Action<object?>? simpleAction2 = null,
        Func<bool>? canExecute1 = null, Func<object?, bool>? canExecute2 = null,
        string functionName = "")
    {
        _model = model;
        _simpleAsync1 = simpleAsync1;
        _simpleAsync2 = simpleAsync2;
        _simpleAction1 = simpleAction1;
        _simpleAction2 = simpleAction2;
        _canExecute1 = canExecute1;
        _canExecute2 = canExecute2;
        _functionName = functionName;
        if (simpleAsync1 is not null && simpleAsync2 is not null)
        {
            throw new CustomBasicException("Cannot have both simple async functions.  Choose one");
        }
        if (simpleAction1 is not null && simpleAction2 is not null)
        {
            throw new CustomBasicException("Cannot have both simple methods.   Choose one");
        }
    }
    protected void HookUpNotifiers()
    {
        int count = 0;
        if (_simpleAsync1 is not null)
        {
            count++;
            _hasMethodParameters = false;
            _isAsync = true;
        }
        if (_simpleAsync2 is not null)
        {
            count++;
            _hasMethodParameters = true;
            _isAsync = true;
        }
        if (_simpleAction1 is not null)
        {
            count++;
            _hasMethodParameters = false;
            _isAsync = false;
        }
        if (_simpleAction2 is not null)
        {
            count++;
            _hasMethodParameters = true;
            _isAsync = false;
        }
        if (_canExecute2 is not null)
        {
            _hasCanParameters = true;
        }
        if (count is not 1)
        {
            throw new CustomBasicException("Needs at least one method.  Otherwise, don't create the command");
        }
        if (_canExecute1 is not null && _canExecute2 is not null)
        {
            throw new CustomBasicException("Cannot have both canexecute functions");
        }
        if (_hasMethodParameters == false && _canExecute2 is not null)
        {
            throw new CustomBasicException("Cannot use CanExecute with parameters if the main function has no parameters");
        }
        AddCommand();
        if (_canExecute1 is null && _canExecute2 is null)
        {
            return; //because there was no canexecute.
        }
        if (_model is INotifyCanExecuteChanged notifier)
        {
            notifier.CanExecuteChanged += Notifier_CanExecuteChanged;
        }
    }
    public void ReportCanExecuteChange()
    {
        CanExecuteChanged?.Invoke(this, new EventArgs());
    }
    private void Notifier_CanExecuteChanged(object sender, CanExecuteChangedEventArgs e)
    {
        if (_functionName == "")
        {
            throw new CustomBasicException("No canexecute function was found.  Should not have raised this.  Rethink");
        }
        if (e.Name == _functionName)
        {
            ReportCanExecuteChange();
        }
    }
    protected bool ParentCanExecute(object? parameter)
    {
        if (_canExecute1 is not null)
        {
            return _canExecute1.Invoke();
        }
        if (_canExecute2 is not null)
        {
            return _canExecute2.Invoke(parameter);
        }
        return true; //if there is none, then let it happen.
    }
    public async virtual Task ExecuteAsync(object? parameter)
    {
        if (ParentCanExecute(parameter) == false)
        {
            return;
        }
        _isExecuting = true;
        await ChangeBlazorStateAsync();
        if (_isAsync == false)
        {
            if (_hasMethodParameters == false)
            {
                _simpleAction1!.Invoke();
            }
            else
            {
                _simpleAction2!.Invoke(parameter);
            }
        }
        else
        {
            if (_hasMethodParameters == false)
            {
                await _simpleAsync1!.Invoke();
            }
            else
            {
                await _simpleAsync2!.Invoke(parameter);
            }
        }
        _isExecuting = false;
        await ChangeBlazorStateAsync();
    }
    protected async Task ChangeBlazorStateAsync()
    {
        if (UpdateBlazor is not null)
        {
            UpdateBlazor.Invoke();
            await Task.Delay(1); //so blazor can update.
        }
    }
}