namespace BasicGameFrameworkLibrary.Core.Extensions;

public static class CommandExtensions
{
    public static bool CanExecuteBasics(this CommandContainer command)
    {
        if (command.IsExecuting == true || command.Processing)
        {
            return false;
        }
        return true;
    }
}