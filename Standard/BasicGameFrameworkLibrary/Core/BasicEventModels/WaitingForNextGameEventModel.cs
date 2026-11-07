namespace BasicGameFrameworkLibrary.Core.BasicEventModels;
public class WaitingForNextGameEventModel(string hostName)
{
    public string HostName { get; } = hostName;
}