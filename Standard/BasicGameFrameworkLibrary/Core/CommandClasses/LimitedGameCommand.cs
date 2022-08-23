namespace BasicGameFrameworkLibrary.Core.CommandClasses;

public class LimitedGameCommand : BasicGameCommand
{
    public LimitedGameCommand(
        IBasicEnableProcess model,
        CommandContainer command,
        Func<Task>? simpleAsync1 = null,
        Func<object?, Task>? simpleAsync2 = null,
        Action? simpleAction1 = null,
        Action<object?>? simpleAction2 = null,
        Func<bool>? canExecute1 = null,
        Func<object?, bool>? canExecute2 = null,
        string functionName = "")
        : base(model, command, simpleAsync1, simpleAsync2, simpleAction1, simpleAction2, canExecute1, canExecute2, functionName)
    {
        BusyCategory = EnumCommandBusyCategory.Limited;
    }
}