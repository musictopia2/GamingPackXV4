﻿namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChooseStockProcesses : IChooseStockProcesses
{
    private readonly LifeBoardGameVMData _model;
    private readonly LifeBoardGameGameContainer _gameContainer;
    public ChooseStockProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    public async Task ChoseStockAsync(int deck) //i think that deck ended up being better after all.
    {
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendAllAsync("chosestock", deck);
        }
        var thisStock = CardsModule.GetStockCard(deck);
        await _gameContainer.ShowCardAsync(thisStock);
        _gameContainer.SingleInfo!.Hand.Add(thisStock);
        if (_gameContainer.SingleInfo.FirstStock == 0)
        {
            _gameContainer.SingleInfo.FirstStock = thisStock.Value;
        }
        else if (_gameContainer.SingleInfo.SecondStock == 0)
        {
            _gameContainer.SingleInfo.SecondStock = thisStock.Value;
        }
        else
        {
            throw new CustomBasicException("Can only have 2 stocks at the most");
        }
        if (_gameContainer.SaveRoot!.EndAfterStock)
        {
            _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        else
        {
            _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    public void LoadStockList()
    {
        _model.HandList!.Text = "List Of Stocks";
        var firstList = _gameContainer.Random.GenerateRandomList(36, 9, 28);
        DeckRegularDict<StockInfo> tempList = new();
        firstList.ForEach(thisItem =>
        {
            tempList.Add(CardsModule.GetStockCard(thisItem));
        });
        var finList = tempList.GetLoadedCards(_gameContainer!.PlayerList!);
        _model.HandList.HandList.Clear(); //try this way now (?)
        _gameContainer.Command.UpdateAll(); //try this now.
        _model.HandList.HandList.ReplaceRange(finList);
        _model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        _model.Instructions = "Choose a stock from the list";
    }
}