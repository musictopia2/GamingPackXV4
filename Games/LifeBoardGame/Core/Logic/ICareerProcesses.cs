namespace LifeBoardGame.Core.Logic;
public interface ICareerProcesses
{
    Task ChoseCareerAsync(int career);
    void LoadCareerList();
}