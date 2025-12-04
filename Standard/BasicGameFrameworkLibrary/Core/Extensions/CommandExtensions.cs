namespace BasicGameFrameworkLibrary.Core.Extensions;
public static class CommandExtensions
{
    extension (CommandContainer command)
    {
        public bool CanExecuteBasics
        {
            get
            {
                if (command.IsExecuting == true || command.Processing)
                {
                    return false;
                }
                return true;
            }
        }
    }
}