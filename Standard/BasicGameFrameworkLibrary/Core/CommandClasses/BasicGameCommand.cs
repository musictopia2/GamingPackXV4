﻿namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public class BasicGameCommand : ParentCommand, IGameCommand
{
    protected new readonly IBasicEnableProcess _model;
    public BasicGameCommand(
        IBasicEnableProcess model,
        CommandContainer command,
        Func<Task>? simpleAsync1 = null,
        Func<object?, Task>? simpleAsync2 = null,
        Action? simpleAction1 = null,
        Action<object?>? simpleAction2 = null,
        Func<bool>? canExecute1 = null,
        Func<object?, bool>? canExecute2 = null,
        string functionName = "") : base(model, simpleAsync1, simpleAsync2, simpleAction1, simpleAction2, canExecute1, canExecute2, functionName)
    {
        CommandContainer = command;
        _model = model;
        HookUpNotifiers();
    }
    public EnumCommandBusyCategory BusyCategory { get; set; }
    protected CommandContainer CommandContainer { get; set; }
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
        if (_model!.CanEnableBasics() == false)
        {
            return false;
        }
        return ParentCanExecute(parameter);
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
    protected virtual void StartExecuting()
    {
        CommandContainer.StartExecuting();
    }
    protected virtual void StopExecuting()
    {
        CommandContainer.StopExecuting();
    }
}