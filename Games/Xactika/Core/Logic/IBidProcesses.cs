namespace Xactika.Core.Logic;
public interface IBidProcesses
{
    Task ProcessBidAsync();
    Task BeginBiddingAsync();
    Task EndBidAsync();
    Task PopulateBidAmountsAsync();
}