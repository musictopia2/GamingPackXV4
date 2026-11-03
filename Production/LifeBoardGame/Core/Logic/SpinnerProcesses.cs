namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class SpinnerProcesses(LifeBoardGameVMData model,
    LifeBoardGameGameContainer gameContainer,
    GameBoardProcesses gameBoard,
    IToast toast
        ) : ISpinnerProcesses
{
    public async Task StartSpinningAsync(SpinnerPositionData position)
    {
        if (gameContainer.CardVisible!.Invoke())
        {
            await gameContainer.HideCardAsync!.Invoke();
        }
        if (gameContainer.SaveRoot!.GameStatus == EnumWhatStatus.NeedChooseHouse || gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedSellBuyHouse)
        {
            gameContainer.SaveRoot.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        gameContainer.Command.UpdateSpecificAction(LifeActionKeys.Spinner);
        await SpinnerAnimationClass.AnimateSpinAsync(position, gameContainer);
        var thisNumber = LifeBoardGameGameContainer.GetNumberSpun(gameContainer.SpinnerPosition);
        gameContainer.SaveRoot.SpinPosition = gameContainer.SpinnerPosition;
        gameContainer.SaveRoot.ChangePosition = 0;
        gameContainer.SaveRoot.NumberRolled = thisNumber;
        await ResumeSpinnerCompletedAsync(); //forgor this part.
    }
    private void EarnProcessFromRoll(int rolled)
    {
        int whoHad;
        if (rolled == 1 || rolled == 10)
        {
            EnumCareerType careerNeeded;
            if (rolled == 1)
            {
                careerNeeded = EnumCareerType.Artist;
            }
            else
            {
                careerNeeded = EnumCareerType.PoliceOfficer;
            }
            whoHad = gameContainer.WhoHadCareerName(careerNeeded);
            if (whoHad > 0 && whoHad != gameContainer.WhoTurn)
            {
                gameContainer.TakeOutExpense(10000);
                gameContainer.CollectMoney(whoHad, 10000);
            }
            if (rolled == 10)
            {
                return;
            }
        }
        whoHad = WhoHadStock(rolled);
        if (whoHad > 0)
        {
            gameContainer.CollectMoney(whoHad, 10000);
        }
    }
    private int WhoHadStock(int whatNum)
    {
        if (whatNum == 0)
        {
            return 0;
        }
        var thisPlayer = gameContainer.PlayerList!.SingleOrDefault(items => items.FirstStock == whatNum || items.SecondStock == whatNum);
        if (thisPlayer == null)
        {
            return 0;
        }
        return thisPlayer.Id;
    }
    private async Task ResumeSpinnerCompletedAsync()
    {
        gameContainer.SaveRoot!.SpinList.Add(gameBoard.NumberRolled);
        EarnProcessFromRoll(gameBoard.NumberRolled);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        if (gameContainer.GameStatus == EnumWhatStatus.NeedToSpin)
        {
            gameContainer.GameStatus = EnumWhatStatus.LastSpin;
        }
        if (gameBoard.NumberRolled == 0)
        {
            BetweenNumbers();
            if (gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                toast.ShowInfoToast("Spin again because it landed between two numbers");
            }
            if (gameContainer.GameStatus == EnumWhatStatus.LastSpin)
            {
                gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
            }
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        await FinishSpinProcessAsync();
        if (gameContainer.CanTradeForBig(false))
        {
            toast.ShowInfoToast("Current Player Is Getting The $100,000 for 2 8s, 9s or 10s spinned in a row for a lucky break for being entertainer");
            await gameContainer.TradeForBigAsync();
            return;
        }
        IMoveProcesses move = gameContainer.Resolver.Resolve<IMoveProcesses>();
        await move.PossibleAutomateMoveAsync();
    }
    private void BetweenNumbers()
    {
        int whoHad = gameContainer.WhoHadCareerName(EnumCareerType.ComputerConsultant);
        if (whoHad > 0 && whoHad != gameContainer.WhoTurn)
        {
            gameContainer.CollectMoney(whoHad, 50000);
        }
    }
    private SpinnerPositionData GetSpinData
    {
        get
        {
            SpinnerPositionData output = new();
            output.CanBetween = gameContainer.Random.NextBool(3); //decided to decrease to 3
            output.HighSpeedUpTo = gameContainer.Random.GetRandomNumber(30, 15);
            output.ChangePositions = gameContainer.Random.GetRandomNumber(120, 60);
            output.SpinnerPosition = gameContainer.SpinnerPosition; //try this way.
            return output;
        }
    }
    public async Task StartSpinningAsync()
    {
        var spin = GetSpinData;
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("spin", spin);
        }
        await StartSpinningAsync(spin);
    }
    private async Task FinishSpinProcessAsync()
    {
        if (gameContainer.GameStatus == EnumWhatStatus.LastSpin)
        {
            gameContainer.RepaintBoard();
        }
        else if (gameContainer.GameStatus == EnumWhatStatus.NeedFindSellPrice || gameContainer.GameStatus == EnumWhatStatus.NeedSellHouse)
        {
            await SoldHouseAsync(gameBoard.NumberRolled);
        }
        //may not matter.  hopefully becomes obvious if it does matter.
    }
    private async Task SoldHouseAsync(int numberRolled)
    {
        var thisHouse = gameContainer.SingleInfo!.HouseCard;
        decimal moneyEarned = thisHouse.SellingPrices[numberRolled];
        gameContainer.SingleInfo!.HouseIsInsured = false;
        model.GameDetails = $"House sold for {moneyEarned.ToCurrency(0)}";
        gameContainer.CollectMoney(gameContainer.WhoTurn, moneyEarned);
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(1);
        }
        gameContainer.SingleInfo.Hand.RemoveSpecificItem(thisHouse);
        PopulatePlayerProcesses.FillInfo(gameContainer.SingleInfo);
        if (gameContainer.GameStatus == EnumWhatStatus.NeedFindSellPrice)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedChooseHouse;
        }
        else
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedChooseRetirement;
        }
    }
}