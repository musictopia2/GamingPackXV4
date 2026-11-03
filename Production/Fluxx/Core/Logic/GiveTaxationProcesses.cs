namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class GiveTaxationProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses processes) : IGiveTaxationProcesses
{
    async Task IGiveTaxationProcesses.GiveCardsForTaxationAsync(IDeckDict<FluxxCardInformation> list)
    {
        if (gameContainer!.CurrentAction == null)
        {
            throw new CustomBasicException("Must have a current action in order to give cards for taxation");
        }
        if (gameContainer.CurrentAction.Deck != EnumActionMain.Taxation)
        {
            throw new CustomBasicException("The current action must be taxation");
        }
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetOtherPlayer();
        if (gameContainer.SingleInfo.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("taxation", list.GetDeckListFromObjectList());
        }
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        list.ForEach(thisCard => thisCard.Drew = true);
        gameContainer.SingleInfo.MainHandList.AddRange(list);
        if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            gameContainer.SortCards!();
        }
        if (gameContainer.LoadPlay == null)
        {
            throw new CustomBasicException("Nobody is loading the play button on main screen for taxation.  Rethink");
        }
        gameContainer.LoadPlay();
        do
        {
            gameContainer.OtherTurn = await gameContainer.PlayerList.CalculateOtherTurnAsync();
            if (gameContainer.OtherTurn == 0)
            {
                gameContainer.CurrentAction = null;
                break;
            }
            gameContainer.SingleInfo = gameContainer.PlayerList.GetOtherPlayer();
            if (gameContainer.SingleInfo.MainHandList.Count > 0)
            {
                break;
            }
        } while (true);
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        await processes.AnalyzeQueAsync();
    }
}