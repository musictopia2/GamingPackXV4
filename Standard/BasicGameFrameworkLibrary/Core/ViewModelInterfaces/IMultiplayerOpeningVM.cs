namespace BasicGameFrameworkLibrary.Core.ViewModelInterfaces;
public interface IMultiplayerOpeningVM<P>
    where P : class, IPlayerItem, new()
{
    void Initialize(); ///this is the startup (no matter what).   including setting up commands.
}