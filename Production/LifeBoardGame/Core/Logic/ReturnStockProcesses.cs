namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class ReturnStockProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer) : IReturnStockProcesses
{
    public void LoadCurrentPlayerStocks()
    {
        if (gameContainer!.SingleInfo!.FirstStock == 0 || gameContainer.SingleInfo.SecondStock == 0)
        {
            throw new CustomBasicException("Must have both stock items.  Otherwise; should do automatically");
        }
        DeckRegularDict<LifeBaseCard> tempList =
        [
            gameContainer.SingleInfo.GetStockCard(gameContainer.SingleInfo.FirstStock),
            gameContainer.SingleInfo.GetStockCard(gameContainer.SingleInfo.SecondStock)
        ];
        model.HandList!.HandList.ReplaceRange(tempList);
        model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        model.HandList.Text = "List Of Stocks";
    }
    public async Task StockReturnedAsync(int deck)
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("stockreturned", deck);
        }
        var thisStock = CardsModule.GetStockCard(deck);
        await gameContainer.ShowCardAsync(thisStock);
        gameContainer.SingleInfo!.Hand.RemoveObjectByDeck(thisStock.Deck);
        if (thisStock.Value == gameContainer.SingleInfo.FirstStock)
        {
            gameContainer.SingleInfo.FirstStock = gameContainer.SingleInfo.SecondStock;
            gameContainer.SingleInfo.FirstStock = 0;
        }
        else if (thisStock.Value == gameContainer.SingleInfo.SecondStock)
        {
            gameContainer.SingleInfo.SecondStock = 0;
        }
        else
        {
            throw new CustomBasicException("Cannot update the stock");
        }
        gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}