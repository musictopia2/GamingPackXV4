namespace GolfCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BeginningProcesses(GolfCardGameGameContainer gameContainer, GolfDelegates delegates, GolfCardGameVMData model) : IBeginningProcesses
{
    async Task IBeginningProcesses.SelectBeginningAsync(int player, DeckRegularDict<RegularSimpleCard> selectList, DeckRegularDict<RegularSimpleCard> unselectList)
    {
        //at this point, run the delegate plus lots of other things.
        if (delegates.LoadMainScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the load main screen.  Rethink");
        }
        if (selectList.Count != 2 || unselectList.Count != 2)
        {
            throw new CustomBasicException("The select and unselect list must contain 2 cards");
        }
        
        gameContainer.SingleInfo = gameContainer.PlayerList![player];
        gameContainer.SingleInfo.MainHandList.ReplaceRange(selectList);
        gameContainer.SingleInfo.TempSets.ReplaceRange(unselectList);
        gameContainer.SingleInfo.FinishedChoosing = true;
        gameContainer.Command.ManuelFinish = true;
        if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            model.HiddenCards1!.ClearBoard();
            model.GolfHand1!.ClearBoard();
        }
        await delegates.LoadMainScreenAsync();
        if (!gameContainer.BasicData!.MultiPlayer)
        {
            gameContainer.SaveRoot!.GameStatus = EnumStatusType.Normal;
            gameContainer.WhoStarts = gameContainer.WhoTurn;
            await gameContainer.StartNewTurnAsync!.Invoke();
            return;
        }
        if (gameContainer.PlayerList.Any(items => !items.FinishedChoosing))
        {
            model.Instructions = "Waiting for the other players to finish choosing the 2 cards";
            gameContainer.Network!.IsEnabled = true;
            return;
        }
        gameContainer.SaveRoot!.GameStatus = EnumStatusType.Normal;
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
}