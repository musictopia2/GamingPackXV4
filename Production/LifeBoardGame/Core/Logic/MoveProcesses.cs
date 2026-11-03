namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class MoveProcesses(LifeBoardGameVMData model,
    LifeBoardGameGameContainer gameContainer,
    GameBoardProcesses gameBoard
        ) : IMoveProcesses, ITwinProcesses
{
    public async Task DoAutomateMoveAsync(int space)
    {
        IBoardProcesses processes = gameContainer.Resolver.Resolve<IBoardProcesses>();
        model!.GameDetails = processes.GetSpaceDetails(space);
        model!.Instructions = "Making Move";
        gameBoard.NewPosition = space;
        if (space == gameBoard.SecondPossiblePosition)
        {
            await gameBoard.AnimateMoveAsync(true);
        }
        else
        {
            await gameBoard.AnimateMoveAsync(false);
        }
        await MoveResultsAsync(space, gameContainer.SingleInfo!);
    }
    private bool TilesLeft
    {
        get
        {
            int howMany = gameContainer.PlayerList!.Sum(items => items.TilesCollected);
            if (howMany > 21)
            {
                throw new CustomBasicException("Only 21 tiles at the most");
            }
            return howMany < 21;
        }
    }
    private bool CanStealTile => gameContainer.PlayerList!.Any(items => items.TilesCollected > 0 && items.LastMove != EnumFinal.CountrySideAcres);
    private async Task MoveResultsAsync(int index, LifeBoardGamePlayerItem thisPlayer)
    {
        var thisSpace = gameContainer!.SpaceList![index - 1];
        PaydayProcessing(thisPlayer);
        decimal newAmount;
        int newPlayer;
        if (thisSpace.GetLifeTile)
        {
            if (TilesLeft)
            {
                LifeBoardGameGameContainer.ObtainLife(thisPlayer);
                gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
            }
            else if (CanStealTile)
            {
                gameContainer.GameStatus = EnumWhatStatus.NeedStealTile;
            }
            else
            {
                gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
            }
        }
        switch (thisSpace.ActionInfo)
        {
            case EnumActionType.CollectPayMoney:
                gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                if (thisSpace.AmountReceived < 0)
                {
                    newAmount = Math.Abs(thisSpace.AmountReceived);
                    if (thisSpace.WhatInsurance == EnumInsuranceType.NeedCar)
                    {
                        if (thisPlayer.CarIsInsured == true)
                        {
                            model!.GameDetails = $"{model.GameDetails}{Constants.VBCrLf} Pay nothing because the player had car insurance";
                        }
                        else
                        {
                            model!.GameDetails = $"{thisSpace.Description}{Constants.VBCrLf}Pay {newAmount.ToCurrency(0)} because the player had no car insurance";
                            gameContainer.TakeOutExpense(newAmount);
                        }
                    }
                    else if (thisSpace.WhatInsurance == EnumInsuranceType.NeedHouse)
                    {
                        if (thisPlayer.HouseIsInsured || thisPlayer.HouseName == "")
                        {
                            model!.GameDetails = $"{model.GameDetails}{Constants.VBCrLf} Pay nothing because the player had house insurance or had no house";
                        }
                        else
                        {
                            model!.GameDetails = $"{thisSpace.Description}{Constants.VBCrLf}Pay {newAmount.ToCurrency(0)} because the player had no house insurance";
                            gameContainer.TakeOutExpense(newAmount);
                        }
                    }
                    else
                    {
                        if (thisSpace.CareerSpace != EnumCareerType.None)
                        {
                            newPlayer = gameContainer.WhoHadCareerName(thisSpace.CareerSpace);
                            if (newPlayer == gameContainer.WhoTurn)
                            {
                                model!.GameDetails = "Owe nothing because the player owns the career";
                            }
                            else if (newPlayer > 0)
                            {
                                var tempPlayer = gameContainer.PlayerList![newPlayer];
                                model!.GameDetails = $"{model.GameDetails}{Constants.VBCrLf} Pay {tempPlayer.NickName} since they own the career";
                                gameContainer.CollectMoney(newPlayer, newAmount);
                            }
                        }
                        gameContainer.TakeOutExpense(newAmount);
                    }
                }
                else
                {
                    gameContainer.CollectMoney(gameContainer.WhoTurn, thisSpace.AmountReceived); //i think
                }

                break;
            case EnumActionType.StartCareer:
                gameContainer.GameStatus = EnumWhatStatus.NeedChooseFirstCareer;
                break;
            case EnumActionType.AttendNightSchool:
                gameContainer.GameStatus = EnumWhatStatus.NeedNight;
                break;
            case EnumActionType.FindNewJob:
                var tempList = thisPlayer.GetCareerList();
                tempList.ForEach(thisCard => thisPlayer.Hand.RemoveSpecificItem(thisCard));
                gameContainer.GameStatus = EnumWhatStatus.NeedNewCareer;
                break;
            case EnumActionType.GetMarried:
                GetMarriedProcess(thisPlayer);
                break;
            case EnumActionType.ObtainLifeTile:
                gameContainer.SaveRoot!.WasMarried = false;
                if (gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
                break;
            case EnumActionType.GetPaid:
                gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                break;
            case EnumActionType.GotBabyBoy:
                GetBaby(thisPlayer, EnumGender.Boy);
                gameContainer.SaveRoot!.WasMarried = false;
                if (gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
                break;
            case EnumActionType.GotBabyGirl:
                GetBaby(thisPlayer, EnumGender.Girl);
                gameContainer.SaveRoot!.WasMarried = false;
                if (gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
                break;
            case EnumActionType.HadTwins:
                if (gameContainer.BasicData!.MultiPlayer && gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData) == false)
                {
                    gameContainer.Network!.IsEnabled = true; //wait to see if they got boys or girls.
                    return;
                }
                var genderList = new BasicList<EnumGender> { EnumGender.Boy, EnumGender.Girl };
                BasicList<EnumGender> newList = [genderList.GetRandomItem(), genderList.GetRandomItem()];
                if (gameContainer.BasicData.MultiPlayer)
                {
                    await gameContainer.Network!.SendAllAsync("twins", newList);
                }
                await GetTwinsAsync(newList);
                return;
            case EnumActionType.MayBuyHouse:
                gameContainer.GameStatus = EnumWhatStatus.NeedChooseHouse;
                break;
            case EnumActionType.MaySellHouse:
                if (thisPlayer.HouseName == "")
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedChooseHouse;
                }
                else
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedSellBuyHouse;
                }
                break;
            case EnumActionType.PayTaxes:
                if (thisSpace.CareerSpace != EnumCareerType.Accountant)
                {
                    throw new CustomBasicException("Only accountants can show up for paying taxes");
                }
                newPlayer = gameContainer.WhoHadCareerName(EnumCareerType.Accountant);
                newAmount = gameContainer.SingleInfo!.TaxesDue;
                if (newPlayer == gameContainer.WhoTurn)
                {
                    model!.GameDetails = "Owe no taxes because the player is the accountant";
                }
                else if (newPlayer != 0)
                {
                    var tempPlayer = gameContainer.PlayerList![newPlayer];
                    model!.GameDetails = $"{model.GameDetails}{Constants.VBCrLf} Pay {tempPlayer.NickName} {newAmount.ToCurrency(0)} in taxes since they are the accountant";
                    gameContainer.CollectMoney(newPlayer, newAmount);
                }
                else
                {
                    model!.GameDetails = $"{model.GameDetails}{Constants.VBCrLf} Pay {newAmount.ToCurrency(0)} in taxes";
                }
                gameContainer.TakeOutExpense(newAmount);
                gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                break;
            case EnumActionType.SpinAgainIfBehind:
                if (gameContainer.WhoTurn != gameBoard!.PlayerInLead)
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
                }
                else
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
                break;
            case EnumActionType.StockCrashed:
                if (thisPlayer.FirstStock > 0 && thisPlayer.SecondStock > 0)
                {
                    gameContainer.GameStatus = EnumWhatStatus.NeedReturnStock;
                }
                else if (thisPlayer.FirstStock > 0 || thisPlayer.SecondStock > 0)
                {
                    model!.GameDetails = $"{model.GameDetails}{Constants.VBCrLf} There was only one stock.  That has been returned automatically.";
                    await AutoReturnAsync();
                    return;
                }
                else
                {
                    model!.GameDetails = $"{model.GameDetails}{Constants.VBCrLf} There was no stocks to return.";
                    gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                }
                break;
            case EnumActionType.StockBoomed:
                gameContainer.GameStatus = EnumWhatStatus.NeedChooseStock;
                gameContainer.SaveRoot!.EndAfterStock = true;
                break;
            case EnumActionType.TradeSalary:
                gameContainer.SaveRoot!.EndAfterSalary = true;
                gameContainer.GameStatus = EnumWhatStatus.NeedTradeSalary;
                break;
            case EnumActionType.WillMissTurn:
                gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
                thisPlayer.MissNextTurn = true;
                break;
            case EnumActionType.WillRetire:
                await RetirementProcessAsync();
                return;
            default:
                throw new CustomBasicException("Move Results Status Not Found");
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task PossibleAutomateMoveAsync()
    {
        if (gameContainer.GameStatus != EnumWhatStatus.NeedToSpin && gameContainer.GameStatus != EnumWhatStatus.LastSpin)
        {
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (gameBoard.FirstPossiblePosition > 0 && gameBoard.SecondPossiblePosition > 0)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToChooseSpace;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (gameBoard.SecondPossiblePosition > 0)
        {
            throw new CustomBasicException("Can't have a second possible option but not the first one");
        }
        await DoAutomateMoveAsync(gameBoard.FirstPossiblePosition);
    }
    private void PaydayProcessing(LifeBoardGamePlayerItem thisPlayer)
    {
        if (gameBoard.PayDaysPassed == 0)
        {
            return;
        }
        decimal amountPaid = thisPlayer.Salary;
        gameContainer.CollectMoney(thisPlayer.Id, amountPaid * gameBoard.PayDaysPassed);
        gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
    }
    private void GetBaby(LifeBoardGamePlayerItem thisPlayer, EnumGender gender)
    {
        thisPlayer.ChildrenList.Add(gender);
        gameContainer.RepaintBoard();
    }
    public async Task GetTwinsAsync(BasicList<EnumGender> twinList)
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        twinList.ForEach(thisTwin => GetBaby(gameContainer.SingleInfo, thisTwin));
        gameContainer.SaveRoot!.WasMarried = false;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private void GetMarriedProcess(LifeBoardGamePlayerItem thisPlayer)
    {
        thisPlayer.Married = true;
        if (gameContainer.GameStatus != EnumWhatStatus.NeedStealTile)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        gameContainer.RepaintBoard();
        gameContainer.SaveRoot!.WasMarried = true;
    }
    private async Task AutoReturnAsync()
    {
        StockInfo thisStock;
        if (gameContainer.SingleInfo!.FirstStock > 0)
        {
            thisStock = (StockInfo)gameContainer.SingleInfo.GetStockCard(gameContainer.SingleInfo.FirstStock);
            gameContainer.SingleInfo.FirstStock = 0;
        }
        else if (gameContainer.SingleInfo.SecondStock > 0)
        {
            thisStock = (StockInfo)gameContainer.SingleInfo.GetStockCard(gameContainer.SingleInfo.SecondStock);
            gameContainer.SingleInfo.SecondStock = 0;
        }
        else
        {
            throw new CustomBasicException("There is nothing to return");
        }
        gameContainer.SingleInfo.Hand.RemoveSpecificItem(thisStock);
        await gameContainer!.ShowCardAsync(thisStock);
        gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task RetirementProcessAsync()
    {
        var careerList = gameContainer.SingleInfo!.GetCareerList();
        careerList.ForEach(thisCard =>
        {
            gameContainer.SingleInfo!.Hand.RemoveSpecificItem(thisCard);
        });
        var thisSalary = gameContainer.SingleInfo!.GetSalaryCard();
        gameContainer.SingleInfo!.Hand.RemoveSpecificItem(thisSalary);
        gameContainer.SingleInfo.HouseIsInsured = false;
        gameContainer.SingleInfo.CarIsInsured = false;
        gameContainer.SingleInfo.Salary = 0;
        RepayLoans();
        PopulatePlayerProcesses.FillInfo(gameContainer.SingleInfo);
        if (gameContainer.SingleInfo.HouseName == "")
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedChooseRetirement;
        }
        else
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedSellHouse;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private void RepayLoans()
    {
        if (gameContainer.SingleInfo!.Loans == 0)
        {
            return;
        }
        var numberLoans = gameContainer.SingleInfo.Loans / 20000;
        gameContainer.SingleInfo.Loans = 0;
        gameContainer.TakeOutExpense(numberLoans * 25000);
    }
}