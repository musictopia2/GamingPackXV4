namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public class ControlCommand : ParentCommand, IGameCommand
{
    protected new readonly IControlObservable _model;
    public ControlCommand(IControlObservable model,
        CommandContainer command,
        Func<Task>? simpleAsync1 = null,
        Func<object?, Task>? simpleAsync2 = null,
        Action? simpleAction1 = null,
        Action<object?>? simpleAction2 = null,
        Func<bool>? canExecute1 = null,
        Func<object?, bool>? canExecute2 = null,
        string functionName = "") : base(model, simpleAsync1, simpleAsync2, simpleAction1, simpleAction2, canExecute1, canExecute2, functionName)
    {
        _model = model;
        CommandContainer = command;
        HookUpNotifiers();
    }
    protected virtual void StartExecuting()
    {
        CommandContainer.StartExecuting();
    }
    protected virtual void StopExecuting()
    {
        CommandContainer.StopExecuting();
    }
    public EnumCommandBusyCategory BusyCategory { get; set; }
    protected CommandContainer CommandContainer { get; set; }
    protected override void AddCommand()
    {
        BusyCategory = EnumCommandBusyCategory.Limited;
        CommandContainer.AddCommand(this);
        if (_canExecute1 is not null || _canExecute2 is not null)
        {
            throw new CustomBasicException("Control commands don't have canexecute because something else happens instead");
        }
    }
    public bool CanExecute(object? parameter)
    {
        if (InProgressHelpers.Reconnecting)
        {
            return false;
        }
        if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
        {
            return false;
        }
        if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
        {
            return false;
        }
        return _model.CanExecute();
    }
    public override async Task ExecuteAsync(object? parameter)
    {
        if (CanExecute(parameter) == false)
        {
            return;
        }
        StartExecuting();
        await base.ExecuteAsync(parameter);
        StopExecuting();
    }
}