namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChoosePlayerProcesses : IChoosePlayerProcesses
{
    private readonly PaydayGameContainer _gameContainer;
    private readonly PaydayVMData _model;
    public ChoosePlayerProcesses(PaydayGameContainer gameContainer, PaydayVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    void IChoosePlayerProcesses.LoadPlayerList()
    {
        BasicList<string> tempList = _gameContainer.PlayerList!.Where(items => items.Id != _gameContainer.WhoTurn).Select(x => x.NickName).ToBasicList();
        _model.AddPopupLists(tempList);
    }
    private PaydayPlayerItem PlayerChosen()
    {
        return _gameContainer.PlayerList!.Where(items => items.NickName == _model.PopUpChosen).Single();
    }
    async Task IChoosePlayerProcesses.ProcessChosenPlayerAsync()
    {
        await _gameContainer.StartProcessPopUpAsync(_model);
        _gameContainer.SaveRoot.OutCards.Add(_gameContainer.SaveRoot.CurrentMail!);
        var thisPlayer = PlayerChosen();
        _gameContainer.SaveRoot.CurrentMail = new MailCard();
        MailCard thisMail = _model.MailPile.GetCardInfo();
        if (thisMail.MailType == EnumMailType.MadMoney)
        {
            _gameContainer.SingleInfo!.MoneyHas += Math.Abs(thisMail.AmountReceived);
            thisPlayer.ReduceFromPlayer(Math.Abs(thisMail.AmountReceived));
        }
        else
        {
            _gameContainer.SingleInfo!.ReduceFromPlayer(Math.Abs(thisMail.AmountReceived));
            thisPlayer.MoneyHas += Math.Abs(thisMail.AmountReceived);
        }
        _gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
}