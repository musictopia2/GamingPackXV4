namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class TradeSalaryProcesses(LifeBoardGameVMData model,
    LifeBoardGameGameContainer gameContainer,
    IToast toast
        ) : ITradeSalaryProcesses
{
    private string ComputerTradeWith
    {
        get
        {
            decimal maxSalary = gameContainer.SingleInfo!.Salary;
            var tempList = model.PlayerPicker!.TextList.Select(items => items.DisplayText).ToBasicList(); //hopefully this simple.
            var nextList = tempList.Select(items => gameContainer.PlayerList![items]).ToBasicList();
            if (nextList.Count == 0)
            {
                return ""; //because nobody to even trade with.
            }
            var maxOne = nextList.OrderByDescending(items => items.Salary).First();
            if (maxOne.Salary < maxSalary)
            {
                return "";
            }
            return maxOne.NickName;
        }
    }
    public async Task ComputerTradeAsync()
    {
        string name = ComputerTradeWith;
        if (name == "")
        {
            if (gameContainer.CanSendMessage())
            {
                await gameContainer.Network!.SendEndTurnAsync();
            }
            await gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        await TradedSalaryAsync(name);
    }
    public void LoadOtherPlayerSalaries()
    {
        var tempList = gameContainer!.PlayerList!.AllPlayersExceptForCurrent();
        var newList = tempList.GetSalaryList();
        gameContainer.AddPlayerChoices(newList);
    }
    public async Task TradedSalaryAsync(string player)
    {
        if (player == "")
        {
            toast.ShowUserErrorToast("Player cannot be blank.  Rethink");
        }
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary && gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("tradedsalary", player);
        }
        model.PlayerPicker.ShowOnlyOneSelectedItem(player);
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(.75);
        }
        var thisPlayer = gameContainer.PlayerList[player];
        SalaryInfo firstSalary;
        SalaryInfo secondSalary;
        firstSalary = thisPlayer.GetSalaryCard();
        secondSalary = gameContainer.SingleInfo.GetSalaryCard();
        gameContainer.SingleInfo.Salary = firstSalary.PayCheck;
        thisPlayer.Salary = secondSalary.PayCheck;
        thisPlayer.Hand.ReplaceItem(firstSalary, secondSalary);
        gameContainer.SingleInfo.Hand.ReplaceItem(secondSalary, firstSalary);
        PopulatePlayerProcesses.FillInfo(gameContainer.SingleInfo);
        PopulatePlayerProcesses.FillInfo(thisPlayer);
        if (gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        else if (gameContainer.GameStatus == EnumWhatStatus.LastSpin)
        {
            IMoveProcesses move = gameContainer.Resolver.Resolve<IMoveProcesses>(); //do this way to stop the overflows.
            await move.PossibleAutomateMoveAsync();
            return;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}
