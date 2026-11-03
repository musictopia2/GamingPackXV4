namespace LifeBoardGame.Core.Logic;
public interface ISpinnerProcesses
{
    Task StartSpinningAsync(SpinnerPositionData position);
    Task StartSpinningAsync();
}