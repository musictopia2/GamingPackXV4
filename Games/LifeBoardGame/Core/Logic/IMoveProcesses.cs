namespace LifeBoardGame.Core.Logic;
public interface IMoveProcesses
{
    Task PossibleAutomateMoveAsync();
    Task DoAutomateMoveAsync(int space);
}