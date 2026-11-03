namespace Fluxx.Core.Logic;
public interface IAnalyzeProcesses
{
    Task AnalyzeQueAsync();
    void AnalyzeRules();
}