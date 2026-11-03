namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class YardSaleProcesses(PaydayGameContainer gameContainer, IDealProcesses dealProcesses) : IYardSaleProcesses
{
    async Task IYardSaleProcesses.FinishYardSaleAsync()
    {
        await FinishYardSaleAsync();
    }
    private async Task FinishYardSaleAsync()
    {
        gameContainer.SaveRoot!.YardSaleDealCard = gameContainer.SaveRoot.DealListLeft.First();
        gameContainer.SaveRoot.DealListLeft.RemoveFirstItem();
        var command = gameContainer.Command;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    async Task IYardSaleProcesses.ProcessYardSaleAsync()
    {
        await dealProcesses.ProcessDealAsync(true);
        if (gameContainer.SaveRoot!.DealListLeft.Count == 0)
        {
            bool rets;
            rets = await dealProcesses.ProcessShuffleDealsAsync();
            if (rets == false)
            {
                return;
            }
            if (gameContainer.BasicData!.MultiPlayer == true)
            {
                await gameContainer.Network!.SendAllAsync("finishyardsale");
            }
        }
        await FinishYardSaleAsync();
    }
}