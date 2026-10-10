namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public abstract class SimpleControlObservable : IControlObservable
{
    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (CanEnableFirst() == false)
            {
                value = false;
            }
            if (SetProperty(ref _isEnabled, value))
            {
                EnableChange();
            }
        }
    }
    public EnumCommandBusyCategory BusyCategory { get; set; }
        = EnumCommandBusyCategory.None;
    public Action? EnabledChanged { get; set; }
    public bool AlwaysDisabled { set; private get; }
    protected CommandContainer CommandContainer;
    protected Func<bool>? _customFunction;
    protected bool _useSpecial;
    protected IBasicEnableProcess? _enableProcess;

    private IEnableAlways? _alwaysProcess;
    public virtual void SendEnableProcesses(IBasicEnableProcess nets, Func<bool> fun)
    {
        SendFunction(fun);
        _useSpecial = true;
        _enableProcess = nets;
    }

    public void SendFunction(Func<bool> fun)
    {
        _useSpecial = true;
        _customFunction = fun;
    }

    public void SendAlwaysEnable(IEnableAlways always)
    {
        _alwaysProcess = always;
        BusyCategory = EnumCommandBusyCategory.Limited;
        SetCommandsLimited();
    }
    public SimpleControlObservable(CommandContainer container)
    {
        CommandContainer = container;
        CommandContainer.AddControl(this);
    }
    public void ManualChange()
    {
        EnableChange();
    }
    protected virtual bool CanEnableFirst()
    {
        return AlwaysDisabled == false;
    }
    protected abstract void EnableChange();

    protected virtual bool SpecialEnable()
    {
        return true;
    }
    protected virtual void SetCommandsLimited() { }
    bool IControlObservable.CanExecute()
    {
        if (IsEnabled == false)
        {
            return false;
        }

        return SpecialEnable();
    }
    public void ReportCanExecuteChange()
    {
        try
        {
            IsEnabled = true;
            if (CommandContainer.IsExecuting &&
                BusyCategory == EnumCommandBusyCategory.None)
            {
                IsEnabled = false;
                return;
            }
            if (CommandContainer.Processing &&
                BusyCategory == EnumCommandBusyCategory.Limited)
            {
                IsEnabled = false;
                return;
            }
            if (_useSpecial == false)
            {
                IsEnabled = AlwaysDisabled == false;
                return;
            }
            if (_alwaysProcess is not null)
            {
                IsEnabled = _alwaysProcess.CanEnableAlways();
                return;
            }
            if (_enableProcess is not null &&
                _enableProcess.CanEnableBasics() == false)
            {
                IsEnabled = false;
                return;
            }
            IsEnabled = _customFunction!();
        }
        finally
        {
            NotifyStateChanged();
        }
    }
    private void NotifyStateChanged()
    {
        EnabledChanged?.Invoke();
    }
}