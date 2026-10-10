namespace BasicGameFrameworkLibrary.Blazor.Bootstrappers;
public interface IGameBootstrapper
{
    Task InitializationTask { get; }
}