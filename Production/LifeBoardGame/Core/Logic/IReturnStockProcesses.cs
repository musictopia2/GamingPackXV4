namespace LifeBoardGame.Core.Logic;
public interface IReturnStockProcesses
{
    Task StockReturnedAsync(int stock);
    void LoadCurrentPlayerStocks();
}