namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class TradeSalaryProcesses : ITradeSalaryProcesses
{
    private readonly LifeBoardGameVMData _model;
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public TradeSalaryProcesses(LifeBoardGameVMData model, 
        LifeBoardGameGameContainer gameContainer,
        IToast toast
        )
    {
        _model = model;
        _gameContainer = gameContainer;
        _toast = toast;
    }
    private string ComputerTradeWith
    {
        get
        {
            decimal maxSalary = _gameContainer.SingleInfo!.Salary;
            var tempList = _model.PlayerPicker!.TextList.Select(items => items.DisplayText).ToBasicList(); //hopefully this simple.
            var nextList = tempList.Select(items => _gameContainer.PlayerList![items]).ToBasicList();
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
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendEndTurnAsync();
            }
            await _gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        await TradedSalaryAsync(name);
    }
    public void LoadOtherPlayerSalaries()
    {
        var tempList = _gameContainer!.PlayerList!.AllPlayersExceptForCurrent();
        var newList = tempList.GetSalaryList();
        _gameContainer.AddPlayerChoices(newList);
    }
    public async Task TradedSalaryAsync(string player)
    {
        if (player == "")
        {
            _toast.ShowUserErrorToast("Player cannot be blank.  Rethink");
        }
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        if (_gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary && _gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendAllAsync("tradedsalary", player);
        }
        _model.PlayerPicker.ShowOnlyOneSelectedItem(player);
        if (_gameContainer.Test.NoAnimations == false)
        {
            await _gameContainer.Delay.DelaySeconds(.75);
        }
        var thisPlayer = _gameContainer.PlayerList[player];
        SalaryInfo firstSalary;
        SalaryInfo secondSalary;
        firstSalary = thisPlayer.GetSalaryCard();
        secondSalary = _gameContainer.SingleInfo.GetSalaryCard();
        _gameContainer.SingleInfo.Salary = firstSalary.PayCheck;
        thisPlayer.Salary = secondSalary.PayCheck;
        thisPlayer.Hand.ReplaceItem(firstSalary, secondSalary);
        _gameContainer.SingleInfo.Hand.ReplaceItem(secondSalary, firstSalary);
        PopulatePlayerProcesses.FillInfo(_gameContainer.SingleInfo);
        PopulatePlayerProcesses.FillInfo(thisPlayer);
        if (_gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary)
        {
            _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        else if (_gameContainer.GameStatus == EnumWhatStatus.LastSpin)
        {
            IMoveProcesses move = _gameContainer.Resolver.Resolve<IMoveProcesses>(); //do this way to stop the overflows.
            await move.PossibleAutomateMoveAsync();
            return;
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
}
