namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class LotteryProcesses : ILotteryProcesses
{
    private readonly PaydayGameContainer _gameContainer;
    private readonly PaydayVMData _model;
    public LotteryProcesses(PaydayGameContainer gameContainer, PaydayVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    bool ILotteryProcesses.CanStartLotteryProcess()
    {
        return _gameContainer.PlayerList!.Count(x => x.ChoseNumber > 0) >= 2;
    }
    void ILotteryProcesses.LoadLotteryList()
    {
        BasicList<int> thisList = Enumerable.Range(1, 6).ToBasicList();
        thisList.RemoveAllOnly(yy => _gameContainer.PlayerList!.Any(xx => xx.ChoseNumber == yy));
        thisList.InsertBeginning(0);
        BasicList<string> tempList = thisList.CastNumberListToStringList();
        _model.AddPopupLists(tempList);
    }
    async Task ILotteryProcesses.ProcessLotteryAsync()
    {
        await _gameContainer.StartProcessPopUpAsync(_model);
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetOtherPlayer();
        _gameContainer.SingleInfo.ChoseNumber = int.Parse(_model.PopUpChosen);
        _gameContainer.Command.UpdateAll();
        if (_gameContainer.Test.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(1);
        }
        if (_gameContainer.OtherTurnProgressAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the other turn process.  Rethink");
        }
        await _gameContainer.OtherTurnProgressAsync.Invoke();
    }
    async Task ILotteryProcesses.RollLotteryAsync()
    {
        decimal newAmount;
        if (_gameContainer.PlayerList!.Any(items => items.ChoseNumber == _model.Cup!.ValueOfOnlyDice))
        {
            var tempList = _gameContainer.PlayerList!.Where(items => items.ChoseNumber > 0).ToBasicList();
            newAmount = tempList.Count * 100;
            newAmount += 1000;
            tempList.ForEach(items => items.ReduceFromPlayer(100));
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.Where(items => items.ChoseNumber == _model.Cup!.ValueOfOnlyDice).Single();
            _gameContainer.SingleInfo.MoneyHas += newAmount;
            _gameContainer.SaveRoot!.Instructions = $"{_gameContainer.SingleInfo.NickName} has won {newAmount.ToCurrency(0)} for the lottery";
            _gameContainer.Command.UpdateAll();
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(2);
            _gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
}