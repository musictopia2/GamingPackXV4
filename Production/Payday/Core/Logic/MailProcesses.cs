namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class MailProcesses(PaydayGameContainer gameContainer,
    PaydayVMData model,
    GameBoardProcesses gameBoard,
    IToast toast
        ) : IMailProcesses
{
    void IMailProcesses.PopulateMails()
    {
        PopulateMails();
    }
    private void PopulateMails()
    {
        var tempList = gameContainer.SingleInfo!.Hand.GetMailOrDealList<MailCard>(EnumCardCategory.Mail);
        model!.CurrentMailList!.HandList.ReplaceRange(tempList);
    }
    async Task IMailProcesses.ProcessMailAsync()
    {
        await ProcessMailAsync();
    }
    private async Task ReshuffleMailAsync()
    {
        var list = gameContainer.SaveRoot!.OutCards.GetMailOrDealList<MailCard>(EnumCardCategory.Mail);
        list.ShuffleList();
        if (gameContainer.BasicData!.MultiPlayer == true)
        {
            await gameContainer.Network!.SendAllAsync("reshufflemaillist", list.GetDeckListFromObjectList());
        }
        await ReshuffleMailAsync(list);
        await ProcessMailAsync();
    }
    private async Task ProcessMailAsync()
    {
        if (gameContainer.SaveRoot!.MailListLeft.Count == 0)
        {
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            bool toreshuffle;
            toreshuffle = gameContainer.ShouldReshuffle;
            if (toreshuffle == false)
            {
                gameContainer.Network!.IsEnabled = true;
                return;
            }
            await ReshuffleMailAsync();
            return;
        }
        var card = gameContainer.SaveRoot.MailListLeft.First();
        gameContainer.SaveRoot.MailListLeft.RemoveFirstItem();
        await ContinueMailProcessesAsync(card);
    }
    private Task ReshuffleMailAsync(DeckRegularDict<MailCard> list)
    {
        toast.ShowInfoToast("Mail is being reshuffled");
        gameContainer.SaveRoot!.MailListLeft = list;
        gameContainer.SaveRoot.OutCards.RemoveAllOnly(items => items.Deck > 24);
        return Task.CompletedTask;
    }
    async Task IMailProcesses.ReshuffleMailAsync(DeckRegularDict<MailCard> list)
    {
        await ReshuffleMailAsync(list);
    }
    void IMailProcesses.SetUpMail()
    {
        BasicList<int> list = gameContainer.Random!.GenerateRandomList(47 + 24, 47, 25);
        if (list.Count != 47)
        {
            throw new CustomBasicException($"Must have 47 mail cards, not {list.Count} cards");
        }
        gameContainer.SaveRoot!.MailListLeft.Clear();
        if (gameContainer.Test.AutoNearEndOfDeckBeginning)
        {
            list = list.Take(4).ToBasicList(); //only 10 cards in mail because near end of deck.
        }
        list.ForEach(index =>
        {
            MailCard thisCard = (MailCard)PaydayGameContainer.GetCard(index);
            gameContainer.SaveRoot.MailListLeft.Add(thisCard);
        });
    }
    private async Task ContinueMailProcessesAsync(MailCard currentMail)
    {
        model.MailPile.AddCard(currentMail);
        gameContainer.SaveRoot!.CurrentMail = currentMail;
        gameContainer.SaveRoot.GameStatus = EnumStatus.ViewMail;
        decimal pays;
        switch (currentMail.MailType)
        {
            case EnumMailType.MadMoney:
                gameContainer.SaveRoot.Instructions = $"Please choose the player to collect {currentMail.AmountReceived.ToCurrency(0)} from";
                gameContainer.SaveRoot.GameStatus = EnumStatus.ChoosePlayer;
                break;
            case EnumMailType.Charity:
                pays = Math.Abs(currentMail.AmountReceived);
                gameContainer.SaveRoot.Instructions = $"{currentMail.Description} for charity. {vb1.VBCrLf} Please pay {pays.ToCurrency(0)}";
                if (gameContainer.Test!.NoAnimations == false)
                {
                    await gameContainer.Delay!.DelaySeconds(2);
                }
                gameContainer.ProcessExpense(gameBoard, pays);
                if (gameContainer.Test.NoAnimations == false)
                {
                    await gameContainer.Delay!.DelaySeconds(1);
                }
                gameContainer.SaveRoot.OutCards.Add(currentMail);
                break;
            case EnumMailType.MoveAhead:
                gameContainer.SaveRoot.GameStatus = EnumStatus.DealOrBuy;
                break;
            case EnumMailType.MonsterCharge:
                pays = Math.Abs(currentMail.AmountReceived);
                gameContainer.SaveRoot.Instructions = $"You received a monster charge of {pays.ToCurrency(0)} {vb1.VBCrLf}Pay at the end of the month.";
                await MailBillsAsync(currentMail);
                break;
            case EnumMailType.Bill:
                pays = Math.Abs(currentMail.AmountReceived);
                gameContainer.SaveRoot.Instructions = $"You received a bill in the amount of {pays.ToCurrency(0)}{vb1.VBCrLf}Pay at the end of the month.";
                await MailBillsAsync(currentMail);
                break;
            case EnumMailType.PayNeighbor:
                pays = Math.Abs(currentMail.AmountReceived);
                gameContainer.SaveRoot.Instructions = $"Please choose the player to pay {pays.ToCurrency(0)} to";
                gameContainer.SaveRoot.GameStatus = EnumStatus.ChoosePlayer;
                break;
            default:
                break;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task MailBillsAsync(MailCard card)
    {
        gameContainer.SaveRoot.GameStatus = EnumStatus.ViewMail;
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        gameContainer.SaveRoot!.CurrentMail = new MailCard();
        gameContainer.SingleInfo!.Hand.Add(card);
        PopulateMails();
        gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(2);
        }
    }
}