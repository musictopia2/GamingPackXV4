namespace BasicGameFrameworkLibrary.Core.GameBoardCollections;
public interface IBasicSpace
{
    void ClearSpace();
    bool IsFilled();
    Vector Vector { get; set; }
}