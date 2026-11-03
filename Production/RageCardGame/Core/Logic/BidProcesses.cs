namespace RageCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BidProcesses(RageCardGameGameContainer gameContainer, RageCardGameVMData model, RageDelgates delgates) : IBidProcesses
{
    public readonly RageCardGameGameContainer GameContainer = gameContainer;
    public async Task LoadBiddingScreenAsync()
    {
        model.BidAmount = -1;
        model.Bid1!.LoadNormalNumberRangeValues(0, GameContainer!.SaveRoot!.CardsToPassOut);
        model.Bid1.UnselectAll();
        if (delgates.LoadBidScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the bidding screen.  Rethink");
        }
        await delgates.LoadBidScreenAsync.Invoke();
    }
    public async Task ProcessBidAsync()
    {
        GameContainer.SingleInfo = GameContainer.PlayerList!.GetWhoPlayer();
        if (GameContainer.SingleInfo.CanSendMessage(GameContainer.BasicData!) == true)
        {
            await GameContainer.Network!.SendAllAsync("bid", model!.BidAmount);
        }
        GameContainer.SingleInfo.BidAmount = model!.BidAmount;
        if (delgates.CloseBidScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is closing bidding screen.  Rethink");
        }
        model.TrickArea1.Visible = true;
        ((IAdvancedTrickProcesses)model.TrickArea1).LoadGame();
        await delgates.CloseBidScreenAsync.Invoke();
        await GameContainer.EndTurnAsync!.Invoke();
    }
}