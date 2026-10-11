namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class LotteryProcesses(PaydayGameContainer gameContainer, PaydayVMData model) : ILotteryProcesses
{
    bool ILotteryProcesses.CanStartLotteryProcess()
    {
        return gameContainer.PlayerList!.Count(x => x.ChoseNumber > 0) >= 2;
    }
    void ILotteryProcesses.LoadLotteryList()
    {
        BasicList<int> thisList = Enumerable.Range(1, 6).ToBasicList();
        thisList.RemoveAllOnly(yy => gameContainer.PlayerList!.Any(xx => xx.ChoseNumber == yy));
        thisList.InsertBeginning(0);
        BasicList<string> tempList = thisList.CastNumberListToStringList();
        model.AddPopupLists(tempList);
    }
    async Task ILotteryProcesses.ProcessLotteryAsync()
    {
        await gameContainer.StartProcessPopUpAsync(model);
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetOtherPlayer();
        gameContainer.SingleInfo.ChoseNumber = int.Parse(model.PopUpChosen);
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        if (gameContainer.OtherTurnProgressAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the other turn process.  Rethink");
        }
        await gameContainer.OtherTurnProgressAsync.Invoke();
    }
    async Task ILotteryProcesses.RollLotteryAsync()
    {
        decimal newAmount;
        if (gameContainer.PlayerList!.Any(items => items.ChoseNumber == model.Cup!.ValueOfOnlyDice))
        {
            var tempList = gameContainer.PlayerList!.Where(items => items.ChoseNumber > 0).ToBasicList();
            newAmount = tempList.Count * 100;
            newAmount += 1000;
            tempList.ForEach(items => items.ReduceFromPlayer(100));
            gameContainer.SingleInfo = gameContainer.PlayerList!.Where(items => items.ChoseNumber == model.Cup!.ValueOfOnlyDice).Single();
            gameContainer.SingleInfo.MoneyHas += newAmount;
            gameContainer.SaveRoot!.Instructions = $"{gameContainer.SingleInfo.NickName} has won {newAmount.ToCurrency(0)} for the lottery";
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(2);
            }

            gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}