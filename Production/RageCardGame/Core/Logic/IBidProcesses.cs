namespace RageCardGame.Core.Logic;
public interface IBidProcesses
{
    Task ProcessBidAsync();
    Task LoadBiddingScreenAsync();
}