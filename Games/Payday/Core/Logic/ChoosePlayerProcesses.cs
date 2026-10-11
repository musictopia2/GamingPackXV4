namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChoosePlayerProcesses(PaydayGameContainer gameContainer, PaydayVMData model) : IChoosePlayerProcesses
{
    void IChoosePlayerProcesses.LoadPlayerList()
    {
        BasicList<string> tempList = gameContainer.PlayerList!.Where(items => items.Id != gameContainer.WhoTurn).Select(x => x.NickName).ToBasicList();
        model.AddPopupLists(tempList);
    }
    private PaydayPlayerItem PlayerChosen()
    {
        return gameContainer.PlayerList!.Where(items => items.NickName == model.PopUpChosen).Single();
    }
    async Task IChoosePlayerProcesses.ProcessChosenPlayerAsync()
    {
        await gameContainer.StartProcessPopUpAsync(model);
        gameContainer.SaveRoot.OutCards.Add(gameContainer.SaveRoot.CurrentMail!);
        var thisPlayer = PlayerChosen();
        gameContainer.SaveRoot.CurrentMail = new MailCard();
        MailCard thisMail = model.MailPile.GetCardInfo();
        if (thisMail.MailType == EnumMailType.MadMoney)
        {
            gameContainer.SingleInfo!.MoneyHas += Math.Abs(thisMail.AmountReceived);
            thisPlayer.ReduceFromPlayer(Math.Abs(thisMail.AmountReceived));
        }
        else
        {
            gameContainer.SingleInfo!.ReduceFromPlayer(Math.Abs(thisMail.AmountReceived));
            thisPlayer.MoneyHas += Math.Abs(thisMail.AmountReceived);
        }
        gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}