namespace Payday.Core.Logic;
public interface IDealProcesses
{
    Task ChoseWhetherToPurchaseDealAsync();
    Task ProcessDealAsync(bool isYardSale);
    void SetUpDeal();
    void PopulateDeals();
    Task ContinueDealProcessesAsync();
    void ReshuffleDeals(DeckRegularDict<DealCard> list);
    Task<bool> ProcessShuffleDealsAsync();
}