namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public interface IPlayerColorChoice : IPlayerItem
{
    bool DidChooseColor { get; }
    void Clear();
}