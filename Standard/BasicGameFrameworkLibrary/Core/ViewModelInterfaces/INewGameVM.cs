namespace BasicGameFrameworkLibrary.Core.ViewModelInterfaces;
public interface INewGameVM : IScreen
{
    bool CanStartNewGame();
    Task StartNewGameAsync();
}