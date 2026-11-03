namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class RotateTradeHandProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses processes, ActionContainer actionContainer) : IRotateTradeHandProcesses
{
    async Task IRotateTradeHandProcesses.RotateHandAsync(EnumDirection direction)
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (gameContainer.SingleInfo.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("rotatehands", direction);
        }
        DeckRegularDict<FluxxCardInformation> oldList;
        DeckRegularDict<FluxxCardInformation> newList;
        int oldTurn = gameContainer.WhoTurn;
        bool oldReverse = gameContainer.SaveRoot!.PlayOrder.IsReversed;
        FluxxPlayerItem oldPlayer;
        gameContainer.SaveRoot.PlayOrder.IsReversed = direction == EnumDirection.Right;
        oldList = gameContainer.SingleInfo.MainHandList.ToRegularDeckDict();
        int count = gameContainer.PlayerList.Count;
        await count.TimesAsync(async x =>
        {
            oldPlayer = gameContainer.SingleInfo;
            gameContainer.WhoTurn = await gameContainer.PlayerList.CalculateWhoTurnAsync();
            gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
            if (oldPlayer.NickName == gameContainer.SingleInfo.NickName)
            {
                throw new CustomBasicException("Its the same player.  A problem");
            }
            newList = gameContainer.SingleInfo.MainHandList.ToRegularDeckDict();
            gameContainer.SingleInfo.MainHandList.ReplaceRange(oldList);
            if (newList.Count == gameContainer.SingleInfo.MainHandList.Count && gameContainer.SingleInfo.MainHandList.Count > 0 && newList.First().Deck == gameContainer.SingleInfo.MainHandList.First().Deck)
            {
                throw new CustomBasicException("Did not rotate the cards");
            }
            oldList = newList;
        });
        gameContainer.SaveRoot.PlayOrder.IsReversed = oldReverse;
        gameContainer.WhoTurn = oldTurn;
        gameContainer.SingleInfo = gameContainer.PlayerList.GetSelf();
        gameContainer.SortCards!();
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        gameContainer!.CurrentAction = null;
        await processes.AnalyzeQueAsync();
    }
    async Task IRotateTradeHandProcesses.TradeHandAsync(int selectedIndex)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("tradehands", selectedIndex);
        }
        int player = actionContainer.GetPlayerIndex(selectedIndex);
        if (player == gameContainer.WhoTurn)
        {
            throw new CustomBasicException("Cannot pick yourself");
        }
        actionContainer.SetUpGoals();
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        var oldList = gameContainer.SingleInfo.MainHandList.ToRegularDeckDict();
        var thisPlayer = gameContainer.PlayerList[player];
        var newList = thisPlayer.MainHandList.ToRegularDeckDict();
        gameContainer.SingleInfo.MainHandList.ReplaceRange(newList);
        thisPlayer.MainHandList.ReplaceRange(oldList);
        int myID = gameContainer.PlayerList.GetSelf().Id;
        if (player == myID || gameContainer.WhoTurn == myID)
        {
            gameContainer.SingleInfo = gameContainer.PlayerList.GetSelf();
            gameContainer.SortCards!();
        }
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        gameContainer!.CurrentAction = null;
        await processes.AnalyzeQueAsync();
    }
}
