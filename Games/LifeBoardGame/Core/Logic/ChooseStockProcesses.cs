namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChooseStockProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer) : IChooseStockProcesses
{
    public async Task ChoseStockAsync(int deck) //i think that deck ended up being better after all.
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("chosestock", deck);
        }
        var thisStock = CardsModule.GetStockCard(deck);
        await gameContainer.ShowCardAsync(thisStock);
        gameContainer.SingleInfo!.Hand.Add(thisStock);
        if (gameContainer.SingleInfo.FirstStock == 0)
        {
            gameContainer.SingleInfo.FirstStock = thisStock.Value;
        }
        else if (gameContainer.SingleInfo.SecondStock == 0)
        {
            gameContainer.SingleInfo.SecondStock = thisStock.Value;
        }
        else
        {
            throw new CustomBasicException("Can only have 2 stocks at the most");
        }
        if (gameContainer.SaveRoot!.EndAfterStock)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        else
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public void LoadStockList()
    {
        model.HandList!.Text = "List Of Stocks";
        var firstList = gameContainer.Random.GenerateRandomList(36, 9, 28);
        DeckRegularDict<StockInfo> tempList = [];
        firstList.ForEach(thisItem =>
        {
            tempList.Add(CardsModule.GetStockCard(thisItem));
        });
        var finList = tempList.GetLoadedCards(gameContainer!.PlayerList!);
        model.HandList.HandList.Clear(); //try this way now (?)
        model.HandList.HandList.ReplaceRange(finList);
        model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        model.Instructions = "Choose a stock from the list";
    }
}