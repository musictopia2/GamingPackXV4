namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class FinalKeeperProcesses(FluxxGameContainer gameContainer,
    IAnalyzeProcesses analyzeProcesses,
    FluxxDelegates delegates,
    IDiscardProcesses discardProcesses
        ) : IFinalKeeperProcesses
{
    async Task IFinalKeeperProcesses.ProcessExchangeKeepersAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            var thisList = new BasicList<KeeperPlayer> { keeperFrom, keeperTo };
            await gameContainer.Network!.SendAllAsync("exchangekeepers", thisList);
        }
        if (delegates.LoadMainScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the main screen when processing exchange keepers.  Rethink");
        }
        await delegates.LoadMainScreenAsync.Invoke();
        KeeperCard fromCard;
        KeeperCard toCard;
        FluxxPlayerItem fromPlayer;
        FluxxPlayerItem toPlayer;
        fromPlayer = gameContainer.PlayerList![keeperFrom.Player];
        toPlayer = gameContainer.PlayerList[keeperTo.Player];
        fromCard = fromPlayer.KeeperList.RemoveObjectByDeck(keeperFrom.Card);
        toCard = toPlayer.KeeperList.RemoveObjectByDeck(keeperTo.Card);
        toCard.IsSelected = false;
        fromCard.IsSelected = false;
        toCard.Drew = false;
        fromCard.Drew = false;
        fromPlayer.KeeperList.Add(toCard);
        toPlayer.KeeperList.Add(fromCard);
        gameContainer!.CurrentAction = null;
        await analyzeProcesses.AnalyzeQueAsync();
    }
    async Task IFinalKeeperProcesses.ProcessTrashStealKeeperAsync(KeeperPlayer thisKeeper, bool isTrashed)
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (gameContainer.SingleInfo.CanSendMessage(gameContainer.BasicData!))
        {
            string status;
            if (isTrashed)
            {
                status = "trashkeeper";
            }
            else
            {
                status = "stealkeeper";
            }
            await gameContainer.Network!.SendAllAsync(status, thisKeeper);
        }
        if (delegates.LoadMainScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the main screen when trashing or stealing keeper.  Rethink");
        }
        await delegates.LoadMainScreenAsync.Invoke();
        gameContainer.SingleInfo = gameContainer.PlayerList[thisKeeper.Player];
        if (isTrashed)
        {
            await discardProcesses.DiscardKeeperAsync(thisKeeper.Card);
            gameContainer!.CurrentAction = null;
            gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
            await analyzeProcesses.AnalyzeQueAsync();
            return;
        }
        var thisCard = gameContainer.SingleInfo.KeeperList.RemoveObjectByDeck(thisKeeper.Card);
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        thisCard.IsSelected = false;
        thisCard.Drew = false;
        gameContainer.SingleInfo.KeeperList.Add(thisCard);
        gameContainer!.CurrentAction = null;
        await analyzeProcesses.AnalyzeQueAsync();
    }
}
