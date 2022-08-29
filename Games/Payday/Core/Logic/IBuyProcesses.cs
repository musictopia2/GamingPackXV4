namespace Payday.Core.Logic;
public interface IBuyProcesses
{
    Task BuyerSelectedAsync(int deck);
    Task ProcessBuyerAsync();
}