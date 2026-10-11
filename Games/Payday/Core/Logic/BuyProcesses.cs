namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class BuyProcesses(PaydayGameContainer gameContainer, PaydayVMData model) : IBuyProcesses
{
    private void RemovePlayerDeal(DealCard thisCard)
    {
        gameContainer.SingleInfo!.Hand.RemoveSpecificItem(thisCard);
        gameContainer.SaveRoot!.OutCards.Add(thisCard);
    }
    async Task IBuyProcesses.BuyerSelectedAsync(int deck)
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (gameContainer.SingleInfo.CanSendMessage(gameContainer.BasicData!) == true)
        {
            await gameContainer.Network!.SendAllAsync("buyerselected", deck);
        }
        var thisDeal = model.CurrentDealList!.HandList.GetSpecificItem(deck);
        thisDeal.IsSelected = false;
        model.CurrentDealList.HandList.ReplaceAllWithGivenItem(thisDeal);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.5);
        }
        RemovePlayerDeal(thisDeal);
        gameContainer.SingleInfo.MoneyHas += thisDeal.Value;
        var tempList = gameContainer.SingleInfo.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
        model.CurrentDealList.HandList.ReplaceRange(tempList);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1.5);
        }
        gameContainer.PopulateDeals(model);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1.5);
        }
        gameContainer.SaveRoot!.GameStatus = EnumStatus.EndingTurn;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    async Task IBuyProcesses.ProcessBuyerAsync()
    {
        if (model!.CurrentDealList!.HandList.Count == 0)
        {
            gameContainer.SaveRoot!.Instructions = "No deals for the bank to buy";
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(3);
            }
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        gameContainer.SaveRoot!.GameStatus = EnumStatus.ChooseBuy;
        gameContainer.SaveRoot.Instructions = "Please select a deal for the bank to buy from you.";
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}