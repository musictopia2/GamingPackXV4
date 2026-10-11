namespace PickelCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BidProcesses(PickelCardGameVMData model, PickelCardGameGameContainer gameContainer, PickelDelegates delegates) : IBidProcesses
{
    public bool CanPass()
    {
        int temps;
        if (gameContainer.WhoTurn == 1)
        {
            temps = 2;
        }
        else if (gameContainer.WhoTurn == 2 && gameContainer.PlayerList!.Count == 3)
        {
            temps = 3;
        }
        else
        {
            temps = 1; //has to do manually because otherwise, can't do it since its a function to determine whether to enable.  that can't be async unfortunately
        }
        if (temps == gameContainer.WhoStarts && gameContainer.SaveRoot!.WonSoFar == 0)
        {
            return false;
        }
        return true;
    }
    public async Task PassBidAsync()
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!) == true)
        {
            await gameContainer.Network!.SendAllAsync("pass");
        }
        await ContinueBidProcessAsync();
    }
    private async Task EndBidAsync()
    {
        if (delegates.CloseBiddingAsync == null)
        {
            throw new CustomBasicException("Nobody is closing the bidding.  Rethink");
        }
        gameContainer.SaveRoot!.GameStatus = EnumStatusList.Normal;
        model.TrickArea1.Visible = true;
        ((IAdvancedTrickProcesses)model.TrickArea1).LoadGame();
        await delegates.CloseBiddingAsync.Invoke();
        var ThisPlayer = gameContainer.PlayerList![gameContainer.SaveRoot.WonSoFar];
        gameContainer.SaveRoot.TrumpSuit = ThisPlayer.SuitDesired;
        if (gameContainer.StartNewTrickAsync == null)
        {
            throw new CustomBasicException("Nobody is handling startnewtrick.  Rethink");
        }
        await gameContainer.StartNewTrickAsync();
    }
    private async Task ContinueBidProcessAsync()
    {
        if (gameContainer.SaveRoot!.HighestBid == 10)
        {
            await EndBidAsync();
            return;
        }
        gameContainer.WhoTurn = await gameContainer.PlayerList!.CalculateWhoTurnAsync();
        if (gameContainer.WhoTurn == gameContainer.WhoStarts)
        {
            await EndBidAsync();
            return;
        }
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        await PopulateBidsAsync();
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
    public async Task ProcessBidAsync()
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!) == true)
        {
            SendBid thisSend = new()
            {
                Bid = model!.BidAmount,
                Suit = model.TrumpSuit
            };
            await gameContainer.Network!.SendAllAsync("bid", thisSend);
        }
        gameContainer.SingleInfo!.BidAmount = model!.BidAmount;
        gameContainer.SingleInfo.SuitDesired = model.TrumpSuit;
        gameContainer.SaveRoot!.HighestBid = model.BidAmount;
        gameContainer.SaveRoot.WonSoFar = gameContainer.WhoTurn;
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.Equals(gameContainer.SingleInfo) == false)
            {
                thisPlayer.SuitDesired = EnumSuitList.None; //because somebody overrided it.
                thisPlayer.BidAmount = 0;
            }
        });
        ResetBids();
        await ContinueBidProcessAsync();
    }
    public void ResetBids()
    {
        model.BidAmount = -1;
        gameContainer!.SaveRoot!.TrumpSuit = EnumSuitList.None;
        model.Bid1!.UnselectAll();
        model.Suit1!.UnselectAll();
    }
    public void SelectBidAndSuit(int bid, EnumSuitList suit)
    {
        model.BidAmount = bid;
        gameContainer!.SaveRoot!.TrumpSuit = suit;
        model.Bid1!.SelectNumberValue(bid);
        model.Suit1!.SelectSpecificItem(suit);
    }
    public async Task PopulateBidsAsync()
    {
        ResetBids();
        //if (_delegates.LoadBiddingAsync == null)
        //{
        //    throw new CustomBasicException("Nobody is loading bids.  Rethink");
        //}
        //await _delegates.LoadBiddingAsync.Invoke();
        if (gameContainer!.SaveRoot!.HighestBid == 0)
        {
            throw new CustomBasicException("The highest bid cannot be 0");
        }
        model.Bid1!.LoadNormalNumberRangeValues(gameContainer.SaveRoot.HighestBid + 1, 10);
        //well see when the shell loads the screen.
    }
}
