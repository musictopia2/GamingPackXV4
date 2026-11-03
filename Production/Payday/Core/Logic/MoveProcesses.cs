namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class MoveProcesses(
    PaydayGameContainer gameContainer,
    PaydayVMData model,
    GameBoardProcesses gameBoard,
    ILotteryProcesses lotteryProcesses,
    IMailProcesses mailProcesses,
    IYardSaleProcesses yardSaleProcesses,
    IBuyProcesses buyProcesses,
    IDealProcesses dealProcesses
        ) : IMoveProcesses
{
    private async Task Pause3Async()
    {
        //may need repaintings (not sure)
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(3);
        }
    }
    private async Task ProcessJackPotAsync()
    {
        decimal newAmount = gameContainer.SaveRoot!.LotteryAmount;
        if (newAmount > 0)
        {
            gameContainer.SaveRoot.Instructions = $"Congratulations, {gameContainer.SingleInfo!.NickName} has won.  {newAmount.ToCurrency(0)} for rolling a six";
            gameContainer.SingleInfo.MoneyHas += newAmount;
            gameBoard!.ClearJackPot();
            if (gameContainer.Test.NoAnimations == false)
            {
                await gameContainer.Delay.DelaySeconds(1);
            }
        }
    }
    private async Task MonthProcessingAsync()
    {
        gameContainer.SingleInfo!.MoneyHas += 3500;
        gameContainer.SaveRoot!.Instructions = "End of month.  Here is your paycheck of $3,500.  Please pay your bills";
        gameContainer.SaveRoot.EndOfMonth = false;
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(2);
        }
        var interest = Math.Ceiling(((decimal)10 / 100) * gameContainer.SingleInfo.Loans);
        if (interest > 0)
        {
            gameContainer.SingleInfo.MoneyHas -= interest;
        }
        gameContainer.SingleInfo.CurrentMonth++;
        PayLoans();
        PayBills();
        mailProcesses.PopulateMails();
        gameContainer.SaveRoot.Instructions = "Bills and loans has been paid.  View the results";
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(1.5);
        }
        if (gameContainer.SaveRoot.EndGame == true)
        {
            gameBoard!.MoveToLastSpace();
            gameContainer.SaveRoot.EndGame = false;
        }
        if (gameContainer.SaveRoot.RemainingMove > 0)
        {
            if (gameContainer.SingleInfo.InGame == false)
            {
                throw new CustomBasicException("Cannot be any remaining move because the player is no longer in the game");
            }
            gameContainer.MonthLabel(model);
            gameContainer.SaveRoot.Instructions = "Finish the move by moving to the highlighted space";
            gameBoard!.HighlightDay(gameContainer.SaveRoot.RemainingMove);
            gameContainer.SaveRoot.GameStatus = EnumStatus.MakeMove;
        }
        else
        {
            gameContainer.SaveRoot.GameStatus = EnumStatus.EndingTurn;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private static int Thousands(decimal amount)
    {
        int x = default;
        if (amount < 1000)
        {
            return 0;
        }
        var NewAmount = 0;
        do
        {
            x += 1;
            NewAmount += 1000;
            if (NewAmount >= amount)
            {
                return x - 1;
            }
            if (x == 5000)
            {
                throw new CustomBasicException("Don't think it can be 5000 times.  If I am wrong; change it");
            }
        }
        while (true);
    }
    private void PayLoans()
    {
        if (gameContainer.SingleInfo!.Loans == 0)
        {
            return;
        }
        if (gameContainer.SingleInfo.MoneyHas >= gameContainer.SingleInfo.Loans)
        {
            gameContainer.SingleInfo.MoneyHas -= gameContainer.SingleInfo.Loans;
            gameContainer.SingleInfo.Loans = 0;
            return;
        }
        int nThousands = Thousands(gameContainer.SingleInfo.MoneyHas);
        if (nThousands == 0)
        {
            return;
        }
        int amountPaid = nThousands * 1000;
        gameContainer.SingleInfo.MoneyHas -= amountPaid;
        gameContainer.SingleInfo.Loans -= amountPaid;
        if (gameContainer.SingleInfo.MoneyHas < 0 || gameContainer.SingleInfo.Loans < 0)
        {
            throw new CustomBasicException("The loans and money a player has must be greater or equal to 0");
        }
    }
    private void PayBills()
    {
        var tempList = model.CurrentMailList!.HandList;
        tempList.ForEach(mail =>
        {
            gameContainer.SingleInfo!.ReduceFromPlayer(Math.Abs(mail.AmountReceived));
            gameContainer.SaveRoot!.OutCards.Add(mail);
            gameContainer.SingleInfo!.Hand.RemoveSpecificItem(mail);
        });
    }
    private void ProcessBirthday()
    {
        decimal moneyGained = (gameContainer.PlayerList!.Count - 1) * 100;
        gameContainer.SingleInfo!.MoneyHas += moneyGained;
        BasicList<PaydayPlayerItem> tempList = gameContainer.PlayerList!.Where(items => items.Id != gameContainer.WhoTurn).ToBasicList();
        tempList.ForEach(x => x.ReduceFromPlayer(100));
    }
    private void ProcessSweepStakes()
    {
        gameContainer.SingleInfo!.MoneyHas += 5000;
    }
    public async Task ResultsOfMoveAsync(int day)
    {
        gameContainer.SaveRoot!.GameStatus = EnumStatus.EndingTurn; //you have to prove otherwise.
        if (model!.Cup!.TotalDiceValue == 6)
        {
            await ProcessJackPotAsync();
        }
        if (day == 31 && gameContainer.SaveRoot.RemainingMove == 0)
        {
            await MonthProcessingAsync();
            return;
        }
        if (gameContainer.SaveRoot.EndOfMonth == true)
        {
            await MonthProcessingAsync();
            return;
        }
        EnumDay details = gameBoard.GetDayDetails(day);
        switch (details)
        {
            case EnumDay.Mail:
                await mailProcesses.ProcessMailAsync();
                break;
            case EnumDay.SweepStakes:
                gameContainer.SaveRoot.Instructions = $"{gameContainer.SingleInfo!.NickName} won $5000.00 dollars in the sweepstakes";
                await Pause3Async();
                ProcessSweepStakes();
                await gameContainer.ContinueTurnAsync!.Invoke();
                break;
            case EnumDay.Deal:
                await dealProcesses.ProcessDealAsync(false);
                break;
            case EnumDay.Buyer:
                await buyProcesses.ProcessBuyerAsync();
                break;
            case EnumDay.Lottery:
                await ProcessLotteryAsync();
                break;
            case EnumDay.YardSale:
                gameContainer.SaveRoot.GameStatus = EnumStatus.ViewYardSale;
                await yardSaleProcesses.ProcessYardSaleAsync();
                break;
            case EnumDay.ShoppingSpree:
                gameContainer.SaveRoot.Instructions = "Shopping Spree.  Please pay $500.00 to the jackpot";
                await Pause3Async();
                gameContainer.ProcessExpense(gameBoard, 500);
                await gameContainer.ContinueTurnAsync!.Invoke();
                break;
            case EnumDay.SkiWeekEnd:
                gameContainer.SaveRoot.Instructions = "Ski Weekend.  Please pay $500.00 to the jackpot";
                await Pause3Async();
                gameContainer.ProcessExpense(gameBoard, 500);
                await gameContainer.ContinueTurnAsync!.Invoke();
                break;
            case EnumDay.HappyBirthday:
                gameContainer.SaveRoot.Instructions = "Happy birthday.  Each player pays you $100.00";
                await Pause3Async();
                ProcessBirthday();
                await gameContainer.ContinueTurnAsync!.Invoke();
                break;
            case EnumDay.CharityConcert:
                gameContainer.SaveRoot.Instructions = "Charity Concert.  Please pay $400.00 to the jackpot";
                await Pause3Async();
                gameContainer.ProcessExpense(gameBoard, 400);
                await gameContainer.ContinueTurnAsync!.Invoke();
                break;
            case EnumDay.RadioContest:
                await ProcessRadioAsync();
                break;
            case EnumDay.Food:
                gameContainer.SaveRoot.Instructions = "Pay Food For The Month.  Please pay $600.00 to the jackpot";
                await Pause3Async();
                gameContainer.ProcessExpense(gameBoard, 600);
                await gameContainer.ContinueTurnAsync!.Invoke();
                break;
            case EnumDay.WalkForCharity:
                gameContainer.OtherTurn = gameContainer.WhoTurn;
                gameContainer.SaveRoot.GameStatus = EnumStatus.RollCharity;
                gameContainer.SaveRoot.Instructions = "Walk for charity.  Roll the dice and pay 100 times the amount you roll to the jackpot";
                await Pause3Async();
                if (gameContainer.OtherTurnProgressAsync == null)
                {
                    throw new CustomBasicException("Nobody is handling other turn progress.  Rethink");
                }
                await gameContainer.OtherTurnProgressAsync.Invoke();
                break;
            default:
                throw new CustomBasicException($"Don't know what to do with {gameContainer.SaveRoot.GameStatus}");
        }
    }
    private async Task ProcessRadioAsync()
    {
        gameContainer.OtherTurn = gameContainer.WhoTurn;
        gameContainer.SaveRoot!.Instructions = "Roll the dice.  The person to roll a 3 wins $1,000";
        gameContainer.SaveRoot.GameStatus = EnumStatus.RollRadio;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task ProcessLotteryAsync()
    {
        gameContainer.OtherTurn = gameContainer.WhoTurn;
        lotteryProcesses.LoadLotteryList();
        gameContainer.SaveRoot!.Instructions = $"Please choose a number between 0 and 6.  Choose 0 to not participate{vb1.VBCrLf} The cost is $100.00.  If you win, then you receive $100.00 times the number of players that participate in addition to the $1,000 the bank donates";
        gameContainer.SaveRoot.GameStatus = EnumStatus.ChooseLottery;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}