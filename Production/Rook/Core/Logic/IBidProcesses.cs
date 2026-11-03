namespace Rook.Core.Logic;
public interface IBidProcesses
{
    Task BeginBiddingAsync();
    Task<bool> CanPassAsync();
    Task PassBidAsync();
    Task ProcessBidAsync();
}