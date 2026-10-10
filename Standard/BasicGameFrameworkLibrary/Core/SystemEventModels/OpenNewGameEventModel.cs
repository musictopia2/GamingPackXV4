namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenNewGameEventModel(NewGameViewModel viewModelUsed)
{
    public NewGameViewModel DataContext => viewModelUsed;
}