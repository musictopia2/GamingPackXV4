namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class DealProcesses(PaydayGameContainer gameContainer
        , PaydayVMData model
        , IToast toast) : IDealProcesses
{
    async Task IDealProcesses.ChoseWhetherToPurchaseDealAsync()
    {
        await gameContainer.StartProcessPopUpAsync(model);
        if (model.PopUpChosen == "Yes")
        {
            //you chose to purchase deal.
            var thisDeal = model.DealPile!.GetCardInfo();
            gameContainer.SingleInfo!.ReduceFromPlayer(Math.Abs(thisDeal.Cost));
            gameContainer.SingleInfo!.Hand.Add(thisDeal);
            gameContainer.PopulateDeals(model);
            if (gameContainer.Test.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(2);
            }
        }
        else
        {
            gameContainer.SaveRoot.OutCards.Add(gameContainer.SaveRoot.CurrentDeal!);
        }
        gameContainer.SaveRoot.CurrentDeal = new DealCard();
        gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    async Task IDealProcesses.ContinueDealProcessesAsync()
    {
        await ContinueDealProcessesAsync();
    }
    private async Task ContinueDealProcessesAsync()
    {
        var thisDeal = gameContainer.SaveRoot!.DealListLeft.First();
        gameContainer.SaveRoot.DealListLeft.RemoveFirstItem();
        string description = thisDeal.Business.Replace("|", " ");
        gameContainer.SaveRoot.Instructions = $"Do you want to buy {description} for {thisDeal.Cost.ToCurrency(0)}?  The value is {thisDeal.Value.ToCurrency(0)}";
        gameContainer.SaveRoot.CurrentDeal = thisDeal;
        model.DealPile!.AddCard(thisDeal);
        gameContainer.SaveRoot.GameStatus = EnumStatus.ChooseDeal;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    void IDealProcesses.PopulateDeals()
    {
        var tempList = gameContainer.SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
        model!.CurrentDealList!.HandList.ReplaceRange(tempList);
    }
    async Task IDealProcesses.ProcessDealAsync(bool isYardSale)
    {
        if (isYardSale == true)
        {
            decimal yardSale = model!.Cup!.TotalDiceValue * 100;
            gameContainer.SingleInfo!.ReduceFromPlayer(yardSale);
            var thisDeal = gameContainer.SaveRoot!.YardSaleDealCard;
            gameContainer.SingleInfo!.Hand.Add(thisDeal!);
            model.DealPile.AddCard(thisDeal!);
            string business = thisDeal!.Business.Replace("|", " ");
            gameContainer.SaveRoot.Instructions = $"Yard sale.  You received {business} for {yardSale.ToCurrency(0)}";
            gameContainer.SaveRoot.GameStatus = EnumStatus.ViewYardSale;
            gameContainer.Command.UpdateSpecificAction(PaydayActions.Yardsale);
            if (gameContainer.Test.NoAnimations == false)
            {
                await gameContainer.Delay.DelaySeconds(2);
            }
            gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
            return;
        }
        if (gameContainer.SaveRoot!.DealListLeft.Count == 0)
        {
            bool rets;
            rets = await ProcessShuffleDealsAsync();
            if (rets == false)
            {
                return;
            }
            if (gameContainer.BasicData.MultiPlayer)
            {
                await gameContainer.Network!.SendAllAsync("continuedealprocesses");
            }
        }
        await ContinueDealProcessesAsync();
    }
    async Task<bool> IDealProcesses.ProcessShuffleDealsAsync()
    {
        return await ProcessShuffleDealsAsync();
    }
    private async Task<bool> ProcessShuffleDealsAsync()
    {
        if (gameContainer.SaveRoot!.DealListLeft.Count != 0)
        {
            throw new CustomBasicException("Should not have ran the processes for reshuffling because not needed");
        }
        bool rets = gameContainer.ShouldReshuffle;
        if (rets == false)
        {
            gameContainer.Network!.IsEnabled = true;
            return false;
        }
        await ReshuffleDealsAsync();
        return true;
    }
    private async Task ReshuffleDealsAsync()
    {
        var thisList = gameContainer.SaveRoot!.OutCards.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
        thisList.ShuffleList();
        if (gameContainer.BasicData!.MultiPlayer == true)
        {
            await gameContainer.Network!.SendAllAsync("reshuffledeallist", thisList.GetDeckListFromObjectList());
        }
        ReshuffleDeals(thisList);
    }
    private void ReshuffleDeals(DeckRegularDict<DealCard> list)
    {
        toast.ShowInfoToast("Deal is being reshuffled");
        gameContainer.SaveRoot!.DealListLeft = list;
        gameContainer.SaveRoot.OutCards.RemoveAllOnly(items => items.Deck <= 24);
    }
    void IDealProcesses.ReshuffleDeals(DeckRegularDict<DealCard> list)
    {
        ReshuffleDeals(list);
    }
    void IDealProcesses.SetUpDeal()
    {
        var list = gameContainer.Random.GenerateRandomList(24);
        if (list.Count != 24)
        {
            throw new CustomBasicException($"Must have 24 deal cards, not {list.Count} cards");
        }
        if (gameContainer.Test.AutoNearEndOfDeckBeginning)
        {
            list = list.Take(4).ToBasicList();
        }
        gameContainer.SaveRoot!.DealListLeft.Clear();
        list.ForEach(index =>
        {
            DealCard thisCard = (DealCard)PaydayGameContainer.GetCard(index);
            gameContainer.SaveRoot.DealListLeft.Add(thisCard);
        });
        gameContainer.SaveRoot.YardSaleDealCard = gameContainer.SaveRoot.DealListLeft.First();
        gameContainer.SaveRoot.DealListLeft.RemoveFirstItem();
    }
}