namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public class OldCommand : ParentCommand, ICustomCommand
{
    public OldCommand(object model,
                      Func<Task>? simpleAsync1 = null,
                      Func<object?, Task>? simpleAsync2 = null,
                      Action? simpleAction1 = null,
                      Action<object?>? simpleAction2 = null,
                      Func<bool>? canExecute1 = null,
                      Func<object?, bool>? canExecute2 = null,
                      string functionName = "") : base(model, simpleAsync1, simpleAsync2, simpleAction1, simpleAction2, canExecute1, canExecute2, functionName)
    {
        HookUpNotifiers();
    }
    public bool CanExecute(object? parameter)
    {
        if (InProgressHelpers.Reconnecting)
        {
            return false;
        }
        if (_isExecuting)
        {
            return false;
        }
        return ParentCanExecute(parameter);
    }
}