namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenPostGameEventModel(IPostGameVM viewModelUsed)
{
    public IPostGameVM DataContext => viewModelUsed;
}