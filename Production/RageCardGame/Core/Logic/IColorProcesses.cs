namespace RageCardGame.Core.Logic;
public interface IColorProcesses
{
    Task ChooseColorAsync();
    Task ColorChosenAsync();
    void ShowLeadColor();
    Task LoadColorListsAsync();
}