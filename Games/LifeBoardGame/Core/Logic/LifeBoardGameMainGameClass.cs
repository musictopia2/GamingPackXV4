namespace LifeBoardGame.Core.Logic;
[SingletonGame]
public class LifeBoardGameMainGameClass
    : SimpleBoardGameClass<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo, EnumColorChoice, int>
    , IBeginningColors<EnumColorChoice, LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    public LifeBoardGameMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        LifeBoardGameVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        LifeBoardGameGameContainer container,
        IBoardProcesses boardProcesses,
        ISpinnerProcesses spinnerProcesses,
        ITwinProcesses twinProcesses,
        IHouseProcesses houseProcesses,
        IChooseStockProcesses chooseStockProcesses,
        IReturnStockProcesses returnStockProcesses,
        ICareerProcesses careerProcesses,
        IBasicSalaryProcesses basicSalaryProcesses,
        ITradeSalaryProcesses tradeSalaryProcesses,
        IStolenTileProcesses stolenTileProcesses,
        GameBoardProcesses gameBoard,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, error, toast)
    {
        _model = model;
        _boardProcesses = boardProcesses;
        _spinnerProcesses = spinnerProcesses;
        _twinProcesses = twinProcesses;
        _houseProcesses = houseProcesses;
        _chooseStockProcesses = chooseStockProcesses;
        _returnStockProcesses = returnStockProcesses;
        _careerProcesses = careerProcesses;
        _basicSalaryProcesses = basicSalaryProcesses;
        _tradeSalaryProcesses = tradeSalaryProcesses;
        _stolenTileProcesses = stolenTileProcesses;
        _gameBoard = gameBoard;
        _gameContainer = container;
    }
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly LifeBoardGameVMData _model;
    private readonly IBoardProcesses _boardProcesses;
    private readonly ISpinnerProcesses _spinnerProcesses;
    private readonly ITwinProcesses _twinProcesses;
    private readonly IHouseProcesses _houseProcesses;
    private readonly IChooseStockProcesses _chooseStockProcesses;
    private readonly IReturnStockProcesses _returnStockProcesses;
    private readonly ICareerProcesses _careerProcesses;
    private readonly IBasicSalaryProcesses _basicSalaryProcesses;
    private readonly ITradeSalaryProcesses _tradeSalaryProcesses;
    private readonly IStolenTileProcesses _stolenTileProcesses;
    private readonly GameBoardProcesses _gameBoard;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        BoardGameSaved();
        SaveRoot.LoadMod(_model);
        if (PlayerList.DidChooseColors() && SaveRoot.GameStatus != EnumWhatStatus.NeedChooseGender)
        {
            _gameBoard.LoadSavedGame();
            _gameContainer.SpinnerPosition = SaveRoot.SpinPosition;
            PlayerList!.ForEach(thisPlayer =>
            {
                var thisList = thisPlayer.Hand.ToRegularDeckDict();
                thisPlayer.Hand.Clear();
                thisList.ForEach(tempCard =>
                {
                    if (tempCard.Deck <= 9)
                    {
                        thisPlayer.Hand.Add(CardsModule.GetCareerCard(tempCard.Deck));
                    }
                    else if (tempCard.Deck <= 18)
                    {
                        thisPlayer.Hand.Add(CardsModule.GetHouseCard(tempCard.Deck));
                    }
                    else if (tempCard.Deck <= 27)
                    {
                        thisPlayer.Hand.Add(CardsModule.GetSalaryCard(tempCard.Deck));
                    }
                    else if (tempCard.Deck <= 36)
                    {
                        thisPlayer.Hand.Add(CardsModule.GetStockCard(tempCard.Deck));
                    }
                    else
                    {
                        throw new CustomBasicException("Must be between 1 and 36");
                    }
                });
            });
            if (SaveRoot.ChangePosition > 0)
            {
                _gameContainer.SpinnerPosition = SaveRoot.ChangePosition;
                _gameContainer.SpinnerRepaint();
            }
            if (SaveRoot.GameStatus == EnumWhatStatus.NeedToEndTurn)
            {
                SingleInfo = PlayerList.GetWhoPlayer();
                if (SingleInfo.Position > 0 && SingleInfo.LastMove == EnumFinal.None)
                {
                    _model.GameDetails = _boardProcesses.GetSpaceDetails(SingleInfo.Position);
                }
            }
        }
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        //InProgressHelpers.MoveInProgress = true; //hopefully this will mean that will show move is in progress if it got to this point no matter what.  since this is much more complex than other games.
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelayMilli(500);
        }
        if (PlayerList.DidChooseColors() == false)
        {
            await base.ComputerTurnAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
        {
            if (_gameContainer.ComputerChooseGenderAsync == null)
            {
                return;
            }
            await _gameContainer.ComputerChooseGenderAsync.Invoke();
            return;
        }
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelayMilli(500);
        }
        switch (SaveRoot.GameStatus)
        {
            case EnumWhatStatus.NeedChooseFirstOption:
                await _boardProcesses.OpeningOptionAsync(EnumStart.Career);
                break;
            case EnumWhatStatus.NeedChooseFirstCareer:
            case EnumWhatStatus.NeedNewCareer:
                await _careerProcesses.ChoseCareerAsync(_model.GetRandomCard);
                break;
            case EnumWhatStatus.NeedChooseStock:
                await _chooseStockProcesses.ChoseStockAsync(_model.GetRandomCard);
                break;
            case EnumWhatStatus.NeedChooseSalary:
                await _basicSalaryProcesses.ChoseSalaryAsync(_model.GetRandomCard);
                break;
            case EnumWhatStatus.NeedToSpin:
                if (SingleInfo!.CarIsInsured == false)
                {
                    await _boardProcesses.PurchaseCarInsuranceAsync();
                    return;
                }
                if (SingleInfo.FirstStock == 0 && SingleInfo.SecondStock == 0)
                {
                    await _boardProcesses.PurchaseStockAsync();
                    return;
                }
                if (SingleInfo.Salary < 80000 && _gameContainer.CanTradeForBig(true))
                {
                    await _boardProcesses.Trade4TilesAsync();
                    return;
                }
                await _spinnerProcesses.StartSpinningAsync();
                break;
            case EnumWhatStatus.NeedReturnStock:
                if (_model.HandList.HandList.Count != 2)
                {
                    throw new CustomBasicException("Must have 2 choices to return.  Otherwise; should have returned automatically");
                }
                await _returnStockProcesses.StockReturnedAsync(_model.GetRandomCard);
                break;
            case EnumWhatStatus.NeedToChooseSpace:
                int firstNumber = _gameBoard.FirstPossiblePosition;
                int secondNumber = _gameBoard.SecondPossiblePosition;
                if (firstNumber == 0)
                {
                    throw new CustomBasicException("The first possible position cannot be 0. Check this out");
                }
                if (secondNumber == 0)
                {
                    throw new CustomBasicException("The second possible position cannot be 0.  Otherwise, should have made move automatically");
                }
                BasicList<int> posList = new() { firstNumber, secondNumber };
                int numberChosen;
                if (Test.DoubleCheck)
                {
                    numberChosen = secondNumber;
                }
                else
                {
                    numberChosen = posList.GetRandomItem();
                }
                await MakeMoveAsync(numberChosen);
                break;
            case EnumWhatStatus.NeedNight:
            case EnumWhatStatus.NeedToEndTurn:
                if (BasicData.MultiPlayer)
                {
                    await Network!.SendEndTurnAsync();
                }
                await EndTurnAsync();
                break;
            case EnumWhatStatus.NeedStealTile:
                await _stolenTileProcesses.ComputerStealTileAsync();
                break;
            case EnumWhatStatus.NeedChooseRetirement:
                await _boardProcesses.RetirementAsync(EnumFinal.MillionaireEstates);
                break;
            case EnumWhatStatus.NeedChooseHouse:
            case EnumWhatStatus.NeedSellBuyHouse:
                await _spinnerProcesses.StartSpinningAsync();
                break;
            case EnumWhatStatus.NeedTradeSalary:
                await _tradeSalaryProcesses.ComputerTradeAsync();
                break;
            default:
                throw new CustomBasicException("Rethink for computer turn");
        }
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SaveRoot!.ImmediatelyStartTurn = true;
        SaveRoot.LoadMod(_model);
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "spin":
                SpinnerPositionData spin = await js.DeserializeObjectAsync<SpinnerPositionData>(content);
                await _spinnerProcesses.StartSpinningAsync(spin);
                return;
            case "gender":
                EnumGender gender = await js.DeserializeObjectAsync<EnumGender>(content);
                if (_gameContainer.SelectGenderAsync == null)
                {
                    throw new CustomBasicException("Nobody is handling the selecting gender.  Rethink");
                }
                await _gameContainer.SelectGenderAsync.Invoke(gender);
                return;
            case "firstoption":
                EnumStart firsts = await js.DeserializeObjectAsync<EnumStart>(content);
                await _boardProcesses.OpeningOptionAsync(firsts);
                return;
            case "chosecareer":
                await _careerProcesses.ChoseCareerAsync(int.Parse(content));
                return;
            case "purchasecarinsurance":
                await _boardProcesses.PurchaseCarInsuranceAsync();
                return;
            case "purchasedstock":
                await _boardProcesses.PurchaseStockAsync();
                return;
            case "tradedlifeforsalary":
                await _boardProcesses.Trade4TilesAsync();
                return;
            case "tradedsalary":
                await _tradeSalaryProcesses.TradedSalaryAsync(content);
                return;
            case "stole":
                await _stolenTileProcesses.TilesStolenAsync(content);
                return;
            case "purchasedhouseinsurance":
                await _boardProcesses.PurchaseHouseInsuranceAsync();
                return;
            case "attendednightschool":
                await _boardProcesses.AttendNightSchoolAsync();
                return;
            case "choseretirement":
                EnumFinal finals = await js.DeserializeObjectAsync<EnumFinal>(content);
                await _boardProcesses.RetirementAsync(finals);
                return;
            case "chosestock":
                await _chooseStockProcesses.ChoseStockAsync(int.Parse(content));
                return;
            case "chosesalary":
                await _basicSalaryProcesses.ChoseSalaryAsync(int.Parse(content));
                return;
            case "stockreturned":
                await _returnStockProcesses.StockReturnedAsync(int.Parse(content));
                return;
            case "chosehouse":
                await _houseProcesses.ChoseHouseAsync(int.Parse(content));
                return;
            case "willsellhouse":
                await _boardProcesses.SellHouseAsync();
                return;
            case "twins":
                BasicList<EnumGender> gList = await js.DeserializeObjectAsync<BasicList<EnumGender>>(content);
                await _twinProcesses.GetTwinsAsync(gList);
                return;
            case "houselist":
                BasicList<int> tempList = await js.DeserializeObjectAsync<BasicList<int>>(content);
                SaveRoot!.HouseList.Clear();
                tempList.ForEach(thisIndex => SaveRoot.HouseList.Add(CardsModule.GetHouseCard(thisIndex)));
                await ContinueTurnAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning
    {
        get
        {
            if (SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
            {
                return false;
            }
            return base.CanMakeMainOptionsVisibleAtBeginning;
        }
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors() && SaveRoot.GameStatus != EnumWhatStatus.NeedChooseGender)
        {
            PrepStartTurn();
            _gameContainer.CurrentSelected = 0;
            if (SingleInfo!.OptionChosen == EnumStart.None)
            {
                SaveRoot.GameStatus = EnumWhatStatus.NeedChooseFirstOption;
            }
            else if (_gameContainer.TeacherChooseSecondCareer)
            {
                SaveRoot.GameStatus = EnumWhatStatus.NeedNewCareer;
                _gameContainer.MaxChosen = 1;
            }
            else
            {
                SaveRoot.GameStatus = EnumWhatStatus.NeedToSpin;
            }
            _gameBoard.NewTurn();
            _gameContainer.SpinnerRepaint();
        }
        await ContinueTurnAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors() && SaveRoot.GameStatus != EnumWhatStatus.NeedChooseGender)
        {
            switch (SaveRoot.GameStatus)
            {
                case EnumWhatStatus.NeedChooseHouse:
                    if (SaveRoot.HouseList.Count == 0)
                    {
                        if (BasicData.MultiPlayer && BasicData.Client)
                        {
                            Network!.IsEnabled = true;
                            return;
                        }
                        var list = _gameContainer.Random.GenerateRandomList(18, 9, 10);
                        SaveRoot.HouseList = list.GetHouseList(PlayerList);
                        if (BasicData.MultiPlayer)
                        {
                            await Network!.SendAllAsync("houselist", SaveRoot.HouseList.GetDeckListFromObjectList());
                        }
                    }
                    _gameBoard.NumberRolled = 0;
                    _houseProcesses.LoadHouseList();
                    break;
                case EnumWhatStatus.NeedToEndTurn:
                    _model!.Instructions = "View results and end turn";
                    break;
                case EnumWhatStatus.NeedChooseStock:
                    _chooseStockProcesses.LoadStockList();
                    break;
                case EnumWhatStatus.NeedSellBuyHouse:
                    _model!.Instructions = "Choose to sell house or spin to not sell house";
                    await _houseProcesses!.ShowYourHouseAsync();
                    _gameBoard.NumberRolled = 0;
                    break;
                case EnumWhatStatus.NeedTradeSalary:
                    _model!.Instructions = $"Choose to trade your salary with someone on list or end turn to keep your current salary{Constants.VBCrLf} Your salary is{SingleInfo!.Salary.ToCurrency(0)}";
                    _tradeSalaryProcesses.LoadOtherPlayerSalaries();
                    break;
                case EnumWhatStatus.NeedFindSellPrice:
                case EnumWhatStatus.NeedSellHouse:
                    await _houseProcesses!.ShowYourHouseAsync();
                    _model!.Instructions = "Spin to find out the selling price for the house";
                    break;
                case EnumWhatStatus.NeedChooseSalary:
                    await _basicSalaryProcesses.LoadSalaryListAsync();
                    break;
                case EnumWhatStatus.NeedReturnStock:
                    _model!.Instructions = "Choose a stock to return";
                    _returnStockProcesses!.LoadCurrentPlayerStocks();
                    break;
                case EnumWhatStatus.NeedStealTile:
                    _stolenTileProcesses.LoadOtherPlayerTiles();
                    break;
                case EnumWhatStatus.NeedChooseRetirement:
                    _gameContainer.CurrentView = EnumViewCategory.EndGame;
                    _model!.Instructions = "Choose either Countryside Acres or Millionaire Estates for retirement";
                    break;
                case EnumWhatStatus.NeedChooseFirstOption:
                    _model!.Instructions = "Choose college or career";
                    break;
                case EnumWhatStatus.NeedToSpin:
                    if (Test!.ImmediatelyEndGame)
                    {
                        await ShowWinAsync();
                        return;
                    }
                    _model!.Instructions = "Spin to take your turn";

                    break;
                case EnumWhatStatus.NeedToChooseSpace:
                    _model!.Instructions = "Decide Which Space";
                    break;
                case EnumWhatStatus.NeedChooseFirstCareer:
                case EnumWhatStatus.NeedNewCareer:
                    _careerProcesses.LoadCareerList();
                    if (_model.HandList!.HandList.Count == 0)
                    {
                        SaveRoot.GameStatus = EnumWhatStatus.NeedChooseSalary;
                        await ContinueTurnAsync();
                        return;
                    }
                    break;
                case EnumWhatStatus.NeedNight:
                    _model!.Instructions = "Decide whether to goto night school or not";
                    break;
                default:
                    break;
            }
        }
        await base.ContinueTurnAsync();
        _gameContainer.Command.UpdateAll();
    }
    public override async Task MakeMoveAsync(int space)
    {
        if (_gameContainer.CanSendMessage())
        {
            await Network!.SendMoveAsync(space);
        }
        IMoveProcesses move = MainContainer.Resolve<IMoveProcesses>();
        await move.DoAutomateMoveAsync(space);
    }
    private async Task FinishTilesAsync()
    {
        _model.Instructions = "None";
        if (SaveRoot.TileList.Count != 25)
        {
            throw new CustomBasicException("Must have 25 tiles");
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            for (int x = 1; x <= thisPlayer.TilesCollected; x++)
            {
                var thisTile = SaveRoot.TileList.First();
                thisPlayer.MoneyEarned += thisTile.AmountReceived;
                SaveRoot.TileList.RemoveFirstItem();
            }
            PopulatePlayerProcesses.FillInfo(thisPlayer);
        });
        SingleInfo = PlayerList.OrderByDescending(items => items.MoneyEarned).First();
        await ShowWinAsync();
    }
    private int WonSoFar(out int secondPlayer)
    {
        secondPlayer = 0;
        var tempList = PlayerList.Where(items => items.LastMove == EnumFinal.MillionaireEstates).OrderByDescending(items => items.NetIncome()).Take(3).ToBasicList();
        if (tempList.Count == 0)
        {
            return 0;
        }
        if (tempList.Count == 1)
        {
            return tempList.Single().Id;
        }
        if (tempList.First().NetIncome() > tempList[1].NetIncome())
        {
            return tempList.First().Id;
        }
        if (tempList.Count > 2)
        {
            if (tempList.First().NetIncome() == tempList.Last().NetIncome())
            {
                return 0; //if 3 way tie, nobody gets.
            }
            tempList.RemoveLastItem();
        }
        if (tempList.First().NetIncome() != tempList.Last().NetIncome())
        {
            throw new CustomBasicException("Does not reconcile");
        }
        secondPlayer = tempList.Last().Id;
        return tempList.First().Id;
    }
    private async Task EndGameProcessAsync()
    {
        int winPlayer = WonSoFar(out int secondPlayer);
        if (winPlayer > 0)
        {
            var thisPlayer = PlayerList![winPlayer];
            if (secondPlayer == 0)
            {
                thisPlayer.TilesCollected += 4;
            }
            else
            {
                thisPlayer.TilesCollected += 2;
                thisPlayer = PlayerList[secondPlayer];
                thisPlayer.TilesCollected += 2;
            }
        }
        await FinishTilesAsync();
    }
    private void CheckSalaries()
    {
        PlayerList.ForEach(player =>
        {
            if (player.Hand.Count(x => x.CardCategory == EnumCardCategory.Salary) > 1)
            {
                throw new CustomBasicException($"Player {player.NickName} had more than one salary.  That is not correct.  Rethink");
            }
        });
    }
    public override async Task EndTurnAsync()
    {
        CheckSalaries();
        SaveRoot.WasNight = false;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        if (WhoTurn > 0)
        {
            await StartNewTurnAsync();
        }
        else
        {
            await EndGameProcessAsync();
        }
    }
    protected override async Task LoadPossibleOtherScreensAsync()
    {
        await base.LoadPossibleOtherScreensAsync();
        if (SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
        {
            if (_gameContainer.ShowGenderAsync == null)
            {
                throw new CustomBasicException("Nobody is handling showing gender.  Rethink");
            }
            await _gameContainer.ShowGenderAsync();
        }
    }
    public override async Task AfterChoosingColorsAsync()
    {
        _gameBoard.ResetBoard(); //has to be after choosing colors after all.  otherwise, the gender gets set to none then it gets hosed (wrong)
        SaveRoot.GameStatus = EnumWhatStatus.NeedChooseGender;
        await Aggregator.PublishAsync(new GenderEventModel());
        await EndTurnAsync();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            if (_gameContainer.ComputerChooseGenderAsync == null)
            {
                throw new CustomBasicException("The computer choose gender was not handed even after loading gender.  Rethink");
            }
            await _gameContainer.ComputerChooseGenderAsync.Invoke();
        }
    }
    internal async Task AfterChoosingGenderAsync()
    {
        if (SaveRoot.GameStatus != EnumWhatStatus.NeedChooseFirstOption)
        {
            throw new CustomBasicException("After choosing gender, something should have set needchoosefirstoption.  Rethink");
        }
        WhoTurn = WhoStarts;
        if (BasicData.MultiPlayer && BasicData.Client)
        {
            Network!.IsEnabled = true;
            return;
        }
        SaveRoot.ImmediatelyStartTurn = false;
        var list = _gameContainer.Random.GenerateRandomList(25);
        SaveRoot.TileList.Clear();
        list.ForEach(x => SaveRoot.TileList.Add(CardsModule.GetTileInfo(x)));
        if (LifeBoardGameGameContainer.StartCollegeCareer)
        {
            SingleInfo = PlayerList.GetWhoPlayer();
            SingleInfo.OptionChosen = EnumStart.College;
            SingleInfo.Position = 10;
        }
        if (BasicData.MultiPlayer)
        {
            await Network!.SendRestoreGameAsync(SaveRoot);
        }
        await StartNewTurnAsync();
    }
}