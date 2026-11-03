namespace Payday.Core.Logic;
public interface IYardSaleProcesses
{
    Task ProcessYardSaleAsync();
    Task FinishYardSaleAsync();
}