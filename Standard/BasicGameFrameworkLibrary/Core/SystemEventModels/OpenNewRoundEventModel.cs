namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenNewRoundEventModel(NewRoundViewModel viewModelUsed)
{
    public NewRoundViewModel DataContext => viewModelUsed;
}