namespace Rook.Core.Logic;
[SingletonGame]
[AutoReset]
public class BidProcesses(RookVMData model, RookGameContainer gameContainer) : IBidProcesses
{
    public async Task BeginBiddingAsync()
    {
        gameContainer.ShowedOnce = true;
        if (gameContainer.SaveRoot!.HighestBidder == 0)
        {
            throw new CustomBasicException("The highest bidder cannot be 0");
        }
        PopulateBids();
        model.CanPass = await CanPassAsync();
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
    private void PopulateBids()
    {
        model.BidChosen = -1;
        if (gameContainer!.SaveRoot!.HighestBidder == 1)
        {
            throw new CustomBasicException("The highest bid cannot be 0");
        }
        if (gameContainer.PlayerList!.Count < 4)
        {
            model.Bid1!.LoadNormalNumberRangeValues(gameContainer.SaveRoot.HighestBidder + 5, 100, 5);
        }
        else
        {
            model.Bid1!.LoadNormalNumberRangeValues(gameContainer.SaveRoot.HighestBidder + 5, 120, 5);
        }
    }
    public async Task<bool> CanPassAsync()
    {
        var temps = await gameContainer.PlayerList!.CalculateWhoTurnAsync();
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
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
        gameContainer.SingleInfo!.Pass = true;
        await ContinueBidProcessAsync();
    }
    public async Task ProcessBidAsync()
    {
        if (model!.BidChosen == -1)
        {
            throw new CustomBasicException("The bid amount cannot be -1");
        }
        if (gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            model.Bid1!.SelectNumberValue(model.BidChosen);
        }
        if (gameContainer.SingleInfo.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("bid", model.BidChosen);
        }
        gameContainer.SaveRoot!.WonSoFar = gameContainer.WhoTurn;
        gameContainer.SingleInfo.BidAmount = model.BidChosen;
        gameContainer.SaveRoot.HighestBidder = model.BidChosen;
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.Id != gameContainer.WhoTurn)
            {
                thisPlayer.BidAmount = 0; //because somebody else won it.
            }
        });
        ResetBids();
        await ContinueBidProcessAsync();
    }
    private async Task ContinueBidProcessAsync()
    {
        if (gameContainer.SaveRoot!.HighestBidder == 100 && gameContainer.PlayerList!.Count < 4)
        {
            await EndBiddingAsync();
            return;
        }
        if (gameContainer.SaveRoot!.HighestBidder == 120)
        {
            await EndBiddingAsync();
            return;
        }
        if (gameContainer.SaveRoot.WonSoFar > 0)
        {
            if (gameContainer.PlayerList!.Count(items => items.Pass == false) == 1) //maybe should have been false (?)
            {
                await EndBiddingAsync();
                return;
            }
        }
        int olds = gameContainer.WhoTurn;
        do
        {
            gameContainer.WhoTurn = await gameContainer.PlayerList!.CalculateWhoTurnAsync();
            gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
            if (gameContainer.SingleInfo.Pass == false)
            {
                break;
            }
        } while (true);
        if (gameContainer.WhoTurn == olds)
        {
            throw new CustomBasicException("Cannot be the same player again");
        }
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        model!.CanPass = await CanPassAsync();
        await BeginBiddingAsync();
    }
    private async Task EndBiddingAsync()
    {
        model!.CanPass = false;
        gameContainer.WhoTurn = gameContainer.SaveRoot!.WonSoFar;
        model.TrickArea1!.NewRound(); //i think.
        if (gameContainer.PlayerList!.Count < 3)
        {
            gameContainer.SaveRoot.GameStatus = EnumStatusList.ChooseTrump;
        }
        else
        {
            gameContainer.SaveRoot.GameStatus = EnumStatusList.SelectNest;
            gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
            gameContainer.SingleInfo!.MainHandList.AddRange(gameContainer.SaveRoot.NestList);
            if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
                gameContainer.SortCards!.Invoke();
            }
        }
        gameContainer.AfterBidding?.Invoke();
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
    private void ResetBids()
    {
        model.BidChosen = -1;
        model.Bid1!.UnselectAll();
    }
}
