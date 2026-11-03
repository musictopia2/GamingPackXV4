namespace PickelCardGame.Core.Logic;
public interface IBidProcesses
{
    Task PassBidAsync();
    bool CanPass();
    Task ProcessBidAsync();
    void ResetBids();
    Task PopulateBidsAsync();
    void SelectBidAndSuit(int bid, EnumSuitList suit);
}