namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public interface ICustomCommand : IAsyncCommand
{
    Action? UpdateBlazor { get; set; }
    void ReportCanExecuteChange();
    object Context { get; }
}