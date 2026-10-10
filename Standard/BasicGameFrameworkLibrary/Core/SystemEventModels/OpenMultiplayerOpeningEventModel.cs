namespace BasicGameFrameworkLibrary.Core.SystemEventModels;
public class OpenMultiplayerOpeningEventModel<P>(MultiplayerOpeningViewModel<P> viewModelUsed)
    where P : class, IPlayerItem, new()
{
    public MultiplayerOpeningViewModel<P> DataContext => viewModelUsed;
}