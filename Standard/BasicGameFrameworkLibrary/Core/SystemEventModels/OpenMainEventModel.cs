namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenMainEventModel(IMainGameVM viewModelUsed)
{
    public IMainGameVM DataContext => viewModelUsed;
}