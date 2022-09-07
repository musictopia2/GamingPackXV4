namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public interface IAdvancedTrickProcesses
{
    Task AnimateWinAsync(int wins);
    void ClearBoard();
    void FirstLoad();
    void LoadGame();
    void HideCards();
}