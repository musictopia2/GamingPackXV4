namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenPromptEventModel(IPromptVM viewModelUsed)
{
    public IPromptVM DataContext => viewModelUsed;
}