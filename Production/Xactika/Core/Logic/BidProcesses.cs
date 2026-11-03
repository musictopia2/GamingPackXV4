namespace Xactika.Core.Logic;
[SingletonGame]
[AutoReset]
public class BidProcesses(XactikaVMData model, XactikaGameContainer gameContainer) : IBidProcesses
{
    async Task IBidProcesses.ProcessBidAsync()
    {
        int bidAmount = model.BidChosen;
        gameContainer.SingleInfo!.BidAmount = bidAmount;
        model!.Bid1!.SelectNumberValue(bidAmount);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        model.Bid1.UnselectAll();
        model.BidChosen = -1;
        await ContinueBidProcessAsync();
    }
    private async Task ContinueBidProcessAsync()
    {
        gameContainer.WhoTurn = await gameContainer.PlayerList!.CalculateWhoTurnAsync(true);
        if (gameContainer.WhoTurn == gameContainer.WhoStarts)
        {
            await EndBidAsync();
            return;
        }
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        await BeginBiddingAsync();
    }
    public async Task EndBidAsync()
    {
        gameContainer.SaveRoot!.GameStatus = EnumStatusList.Normal;
        if (gameContainer.CloseBidding == null)
        {
            throw new CustomBasicException("Nobody is closing the bidding.  Rethink");
        }
        gameContainer.CloseBidding.Invoke();
        if (gameContainer.StartNewTrickAsync == null)
        {
            throw new CustomBasicException("Nobody is starting new trick.  Rethink");
        }
        await gameContainer.StartNewTrickAsync(); //not sure about the turn proceeses (?)
    }
    public async Task BeginBiddingAsync()
    {
        await PopulateBidAmountsAsync();
        gameContainer.ShowTurn!.Invoke();
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
    public async Task PopulateBidAmountsAsync()
    {
        var nextPlayer = await gameContainer.PlayerList!.CalculateWhoTurnAsync(true);
        int nonNumber = -1;
        if (nextPlayer == gameContainer.WhoStarts)
        {
            var total = gameContainer.PlayerList.Where(items => items.BidAmount > -1).Sum(items => items.BidAmount);
            if (total > 8)
            {
                nonNumber = -1;
            }
            else
            {
                nonNumber = 8 - total;
            }
        }
        int x;
        BasicList<int> tempList = [];
        for (x = 0; x <= 8; x++)
        {
            if (x != nonNumber)
            {
                tempList.Add(x);
            }
        }
        model!.Bid1!.UnselectAll();
        model.BidChosen = -1;
        model.Bid1.LoadNumberList(tempList);
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}
