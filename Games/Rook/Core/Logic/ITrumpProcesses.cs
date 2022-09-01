namespace Rook.Core.Logic;
public interface ITrumpProcesses
{
    Task ProcessTrumpAsync();
    void ResetTrumps();
}