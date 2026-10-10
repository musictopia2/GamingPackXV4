namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenRestoreEventModel(RestoreViewModel viewModelUsed)
{
    public RestoreViewModel DataContext => viewModelUsed;
}