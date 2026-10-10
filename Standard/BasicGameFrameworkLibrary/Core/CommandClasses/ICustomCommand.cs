namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public interface ICustomCommand : IAsyncCommand
{
    event Action? CanExecuteChanged;
    void ReportCanExecuteChange();
}