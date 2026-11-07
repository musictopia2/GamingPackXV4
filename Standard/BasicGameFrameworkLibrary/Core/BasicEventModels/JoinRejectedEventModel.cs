namespace BasicGameFrameworkLibrary.Core.BasicEventModels;
public class JoinRejectedEventModel(string reason)
{
    public string Reason { get; } = reason;
}