using System.Diagnostics.CodeAnalysis;

namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BoardProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer, IToast toast) : IBoardProcesses
{
    public bool CanTrade4Tiles => gameContainer.CanTradeForBig(true);
    public bool CanPurchaseCarInsurance => gameContainer.GameStatus == EnumWhatStatus.NeedToSpin && gameContainer.SingleInfo!.CarIsInsured == false;
    public bool CanAttendNightSchool => gameContainer.GameStatus == EnumWhatStatus.NeedNight;
    public bool CanPurchaseHouseInsurance => gameContainer.SingleInfo!.HouseIsInsured == false
        && gameContainer.GameStatus == EnumWhatStatus.NeedToSpin
        && gameContainer.SingleInfo.HouseName != "";
    public bool CanPurchaseStock
    {
        get
        {
            if (gameContainer.GameStatus != EnumWhatStatus.NeedToSpin)
            {
                return false;
            }
            if (gameContainer.SingleInfo!.FirstStock > 0 || gameContainer.SingleInfo.SecondStock > 0)
            {
                return false;
            }
            return true;
        }
    }
    public bool CanSellHouse => gameContainer.GameStatus == EnumWhatStatus.NeedSellBuyHouse; //possibly used wrong status here.
    public bool CanEndTurn
    {
        get
        {
            if (model.Instructions == "Choose one career or end turn and keep your current career")
            {
                return true;
            }
            return gameContainer!.GameStatus == EnumWhatStatus.NeedToEndTurn || gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary || gameContainer.GameStatus == EnumWhatStatus.NeedNight;
        }
    }
    public async Task AttendNightSchoolAsync()
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("attendednightschool");
        }
        gameContainer.SaveRoot!.WasNight = true;
        model.GameDetails = "Paid $20,000 to attend night school to possibly get a better career";
        gameContainer.TakeOutExpense(20000);
        gameContainer.GameStatus = EnumWhatStatus.NeedNewCareer;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task ComputerChoseSpaceAsync(int space)
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendMoveAsync(space);
        }
        IMoveProcesses move = gameContainer.Resolver.Resolve<IMoveProcesses>();
        await move.DoAutomateMoveAsync(space); //hopefully this simple.
    }
    public string GetSpaceDetails(int space)
    {
        var thisSpace = gameContainer!.SpaceList![space - 1];
        decimal newAmount;
        string output;
        switch (thisSpace.ActionInfo)
        {
            case EnumActionType.CollectPayMoney:
                if (thisSpace.AmountReceived < 0)
                {
                    newAmount = Math.Abs(thisSpace.AmountReceived);
                    output = $"{thisSpace.Description}{Constants.VBCrLf}Pay {newAmount.ToCurrency(0)}";
                    if (thisSpace.WhatInsurance != EnumInsuranceType.NoInsurance)
                    {
                        output += Constants.VBCrLf + " if not insured";
                    }
                    return output;
                }
                return thisSpace.Description + Constants.VBCrLf + thisSpace.AmountReceived.ToCurrency(0);
            case EnumActionType.AttendNightSchool:
                return $"Night School.{Constants.VBCrLf} Pay $20,000";
            case EnumActionType.FindNewJob:
                return thisSpace.Description;
            case EnumActionType.GetMarried:
                return "Get Married";
            case EnumActionType.GetPaid:
                return "Pay!" + Constants.VBCrLf + "Day";
            case EnumActionType.GotBabyBoy:
                return "Baby boy!" + Constants.VBCrLf + "Life";
            case EnumActionType.GotBabyGirl:
                return "Baby girl!" + Constants.VBCrLf + "Life";
            case EnumActionType.HadTwins:
                return thisSpace.Description + Constants.VBCrLf + "Life";
            case EnumActionType.MayBuyHouse:
                return "You may BUY A HOUSE" + Constants.VBCrLf + "Draw Deed";
            case EnumActionType.MaySellHouse:
                return "You may sell your house and buy a new one.";
            case EnumActionType.ObtainLifeTile:
                return thisSpace.Description + Constants.VBCrLf + "Life";
            case EnumActionType.PayTaxes:
                return "Taxes due.";
            case EnumActionType.SpinAgainIfBehind:
                return "Spin again if you are not in the lead.";
            case EnumActionType.StartCareer:
                return "CAREER CHOICE";
            case EnumActionType.StockBoomed:
                return "Stock market soars!" + Constants.VBCrLf + "Collect 1 stock.";
            case EnumActionType.StockCrashed:
                return "Stock market crash." + Constants.VBCrLf + "Return 1 stock.";
            case EnumActionType.TradeSalary:
                return "Trade salary card with any player.";
            case EnumActionType.WillMissTurn:
                return thisSpace.Description + Constants.VBCrLf + "Miss next turn.";
            case EnumActionType.WillRetire:
                return "RETIRE" + Constants.VBCrLf + "Go to Countryside Acres" + Constants.VBCrLf + "or Millionaire Estates.";
            default:
                throw new CustomBasicException("No description for " + thisSpace.ActionInfo.ToString());
        }
    }
    public async Task HumanChoseSpaceAsync()
    {
        if (gameContainer.CurrentSelected == 0)
        {
            toast.ShowUserErrorToast("Must choose space to move to");
            return;
        }
        IMoveProcesses move = gameContainer.Resolver.Resolve<IMoveProcesses>();
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendMoveAsync(gameContainer.CurrentSelected);
        }
        await move.DoAutomateMoveAsync(gameContainer.CurrentSelected);
    }
    public async Task OpeningOptionAsync(EnumStart start)
    {
        if (gameContainer.CanSendMessage() == true)
        {
            await gameContainer.Network!.SendAllAsync("firstoption", start);
        }
        gameContainer.SingleInfo!.OptionChosen = start;
        gameContainer.RepaintBoard();
        if (start == EnumStart.College)
        {
            gameContainer.SingleInfo.Loans = 100000;
            gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        else
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedChooseFirstCareer;
        }

        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task PurchaseCarInsuranceAsync()
    {
        if (gameContainer.CanSendMessage() == true)
        {
            await gameContainer.Network!.SendAllAsync("purchasecarinsurance");
        }
        gameContainer.SingleInfo!.CarIsInsured = true;
        gameContainer.TakeOutExpense(5000);
        model.GameDetails = "Paid $5,000 for car insurance.  Now you owe nothing for car damages or car accidents";
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task PurchaseHouseInsuranceAsync()
    {
        if (gameContainer.CanSendMessage() == true)
        {
            await gameContainer.Network!.SendAllAsync("purchasedhouseinsurance");
        }
        decimal amountToPay = gameContainer.SingleInfo!.InsuranceCost();
        gameContainer.TakeOutExpense(amountToPay);
        model.GameDetails = "Paid $5,000 for car insurance.  Now you owe nothing for car damages or car accidents";
        gameContainer.SingleInfo!.HouseIsInsured = true;
        gameContainer.ProcessCommission();
        model!.GameDetails = $"Paid {amountToPay.ToCurrency(0)}.  Now you owe nothing for damages for the house";
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task PurchaseStockAsync()
    {
        if (gameContainer.CanSendMessage() == true)
        {
            await gameContainer.Network!.SendAllAsync("purchasedstock");
        }
        gameContainer.SaveRoot!.EndAfterStock = false;
        gameContainer.TakeOutExpense(50000);
        gameContainer.ProcessCommission();
        model.GameDetails = "Paid $50,000 for stock";
        gameContainer.GameStatus = EnumWhatStatus.NeedChooseStock;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task RetirementAsync(EnumFinal final)
    {
        if (gameContainer.CanSendMessage() == true)
        {
            await gameContainer.Network!.SendAllAsync("choseretirement", final);
        }
        gameContainer.SingleInfo!.LastMove = final;
        gameContainer.RepaintBoard();
        gameContainer.SingleInfo.InGame = false;
        await gameContainer.EndTurnAsync!.Invoke();
    }
    public async Task SellHouseAsync()
    {
        if (gameContainer.CanSendMessage() == true)
        {
            await gameContainer.Network!.SendAllAsync("willsellhouse");
        }
        gameContainer.GameStatus = EnumWhatStatus.NeedFindSellPrice;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public void SpaceDescription(int space)
    {
        gameContainer.CurrentSelected = space;
        model.GameDetails = GetSpaceDetails(space);
        gameContainer.RepaintBoard();
        gameContainer.Command.ReportAll();
    }
    public async Task Trade4TilesAsync()
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("tradedlifeforsalary");
        }
        gameContainer.SingleInfo!.TilesCollected -= 4;
        await gameContainer.TradeForBigAsync(); //i think.
    }
}