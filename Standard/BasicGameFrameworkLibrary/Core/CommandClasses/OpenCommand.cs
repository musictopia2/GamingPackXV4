namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public class OpenCommand(object model,
    CommandContainer command,
    Func<Task>? simpleAsync1 = null,
    Func<object?, Task>? simpleAsync2 = null,
    Action? simpleAction1 = null,
    Action<object?>? simpleAction2 = null,
    Func<bool>? canExecute1 = null,
    Func<object?, bool>? canExecute2 = null,
    string functionName = "") : PlainCommand(model, command, simpleAsync1, simpleAsync2, simpleAction1, simpleAction2, canExecute1, canExecute2, functionName)
{
    protected override void StartExecuting()
    {
        CommandContainer.OpenBusy = true;
    }
    protected override void StopExecuting() { }
    public override bool CanExecute(object? parameter)
    {
        if (InProgressHelpers.Reconnecting)
        {
            return false;
        }
        if (CommandContainer.OpenBusy == true)
        {
            return false;
        }
        return ParentCanExecute(parameter);
    }
}