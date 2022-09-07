namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public interface IPlayerColors : IPlayerItem
{
    bool DidChooseColor { get; }
    void Clear();
}