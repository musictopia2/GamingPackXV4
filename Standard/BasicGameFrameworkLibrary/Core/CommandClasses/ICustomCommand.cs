namespace BasicGameFrameworkLibrary.Core.CommandClasses;

public interface ICustomCommand : IAsyncCommand
{
    Action? UpdateBlazor { get; set; } //this is needed.  this means that blazor can populate this.
    void ReportCanExecuteChange();
    //we needed the context for the commandcontainer though.
    object Context { get; }
}