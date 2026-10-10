namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenBeginningColorEventModel(IBeginningColorViewModel modelUsed)
{
    public IBeginningColorViewModel DataContext => modelUsed;
}
