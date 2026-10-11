namespace LifeBoardGame.Core.ViewModels;
[InstanceGame]
[EventAggregatorFromMemberClass(nameof(Aggregator))]
public partial class LifeBoardGameMainViewModel(CommandContainer commandContainer,
        LifeBoardGameMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        LifeBoardGameGameContainer gameContainer,
        LifeBoardGameVMData model) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
    , IHandleAsync<ShowCardEventModel>, IHandle<EnumWhatStatus>
{
    public LifeBoardGameVMData VMData { get; set; } = model;
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected override void AdditionalInitialize()
    {
        Subscribe();
        if (mainGame.SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
        {
            throw new CustomBasicException("Cannot load main view model because you need to choose gender.  Rethink");
        }
        if (mainGame.PlayerList.Any(x => x.Gender == EnumGender.None))
        {
            throw new CustomBasicException("Cannot load the main screen if sombody did not choose gender.  Rethink");
        }
        gameContainer.HideCardAsync = HideCardAsync;
        gameContainer.CardVisible = () =>ShowCards;
        if (LifeBoardGameGameContainer.StartCollegeCareer && BasicData.GamePackageMode == EnumGamePackageMode.Production)
        {
            throw new CustomBasicException("You cannot start college career because its in production");
        }
        base.AdditionalInitialize();
    }

    public bool ShowCards { get; private set; } = false;
    public bool ShowScores { get; private set; } = false;

    private async Task HideCardAsync()
    {
        if (ShowCards == false)
        {
            return;
        }
        ShowCards = false;
        OnStateChanged();
    }
    private void ShowCard(EnumWhatStatus oldStatus)
    {
        switch (oldStatus)
        {

            case EnumWhatStatus.NeedChooseFirstCareer:
            case EnumWhatStatus.NeedNewCareer:
                CloseCareer();
                LoadShowCard();
                break;
            case EnumWhatStatus.NeedChooseHouse:
            case EnumWhatStatus.NeedSellHouse:
            case EnumWhatStatus.NeedSellBuyHouse:
                CloseHouse();
                LoadShowCard();
                break;
            case EnumWhatStatus.NeedReturnStock:
                CloseReturnStock();
                LoadShowCard();
                break;
            case EnumWhatStatus.NeedChooseStock:
                CloseChooseStock();
                LoadShowCard();
                break;
            case EnumWhatStatus.NeedChooseSalary:
                CloseChooseSalary();
                LoadShowCard();
                break; //sometimes it shows up for a second.  other times, only for a while.
            default:
                LoadShowCard(); //in this case, just show the card.
                break;
        }
    }
    #region Load Screens

    private void LoadScores()
    {
        if (ShowScores)
        {
            return;
        }
        ShowScores = true;
    }

    //private async Task LoadScoreAsync()
    //{
    //    if (ScoreScreen != null)
    //    {
    //        return;
    //    }
    //    ScoreScreen = _resolver.Resolve<LifeScoreboardViewModel>();
    //    await LoadScreenAsync(ScoreScreen);
    //}
    private void LoadCareer()
    {
        if (CareerScreen != null)
        {
            return;
        }
        CareerScreen = Resolver.Resolve<ChooseCareerViewModel>();
    }
    private void LoadChooseSalary()
    {
        if (ChooseSalaryScreen != null)
        {
            return;
        }
        ChooseSalaryScreen = Resolver.Resolve<ChooseSalaryViewModel>();
    }
    private void LoadChooseStock()
    {
        if (ChooseStockScreen != null)
        {
            return;
        }
        ChooseStockScreen = Resolver.Resolve<ChooseStockViewModel>();
    }
    private void LoadReturnStock()
    {
        if (ReturnStockScreen != null)
        {
            return;
        }
        ReturnStockScreen = Resolver.Resolve<ReturnStockViewModel>();
    }
    private void LoadSpinner()
    {
        if (SpinnerScreen != null)
        {
            return;
        }
        SpinnerScreen = Resolver.Resolve<SpinnerViewModel>();
    }
    private void LoadStealTiles()
    {
        if (StealTilesScreen != null)
        {
            return;
        }
        StealTilesScreen = Resolver.Resolve<StealTilesViewModel>();
    }
    private void LoadTradeSalary()
    {
        if (TradeSalaryScreen != null)
        {
            return;
        }
        TradeSalaryScreen = Resolver.Resolve<TradeSalaryViewModel>();
    }
    private void LoadHouse()
    {
        if (HouseScreen != null)
        {
            return;
        }
        HouseScreen = Resolver.Resolve<ChooseHouseViewModel>();
    }
    //for now its static.
    //private static Task LoadBoardAsync() //keep this for now.
    //{
    //    return Task.CompletedTask;
    //}
    private void LoadShowCard()
    {
        if (ShowCards)
        {
            return;
        }
        ShowCards = true;
    }
    #endregion
    #region Screens
    //public LifeScoreboardViewModel? ScoreScreen { get; set; }
    public ChooseCareerViewModel? CareerScreen { get; set; }
    public ChooseSalaryViewModel? ChooseSalaryScreen { get; set; }
    public ChooseStockViewModel? ChooseStockScreen { get; set; }
    public ReturnStockViewModel? ReturnStockScreen { get; set; }
    public SpinnerViewModel? SpinnerScreen { get; set; }
    public StealTilesViewModel? StealTilesScreen { get; set; }
    public TradeSalaryViewModel? TradeSalaryScreen { get; set; }
    public ChooseHouseViewModel? HouseScreen { get; set; }
    //public ShowCardViewModel? ShowCardScreen { get; set; }
    #endregion
    #region Unload Screens

    protected override Task CloseAsync()
    {
        Unsubscribe();
        CloseScore();
        CloseCareer();
        CloseChooseSalary();
        CloseChooseStock();
        CloseReturnStock();
        CloseSpinner();
        CloseStealTiles();
        CloseTradeSalary();
        CloseHouse();
        return base.CloseAsync();
    }

    private void CloseScore()
    {
        if (ShowScores == false)
        {
            return;
        }
        ShowScores = false;
    }
    private void CloseCareer()
    {
        if (CareerScreen == null)
        {
            return;
        }
        CareerScreen = null;
    }
    private void CloseChooseSalary()
    {
        if (ChooseSalaryScreen == null)
        {
            return;
        }
        ChooseSalaryScreen = null;
    }
    private void CloseChooseStock()
    {
        if (ChooseStockScreen == null)
        {
            return;
        }
        ChooseStockScreen = null;
    }
    private void CloseReturnStock()
    {
        if (ReturnStockScreen == null)
        {
            return;
        }
        ReturnStockScreen = null;
    }
    private void CloseSpinner()
    {
        if (SpinnerScreen == null)
        {
            return;
        }
        SpinnerScreen = null;
    }
    private void CloseStealTiles()
    {
        if (StealTilesScreen == null)
        {
            return;
        }
        StealTilesScreen = null;
    }
    private void CloseTradeSalary()
    {
        if (TradeSalaryScreen == null)
        {
            return;
        }
        TradeSalaryScreen = null;
    }
    private void CloseHouse()
    {
        if (HouseScreen == null)
        {
            return;
        }
        HouseScreen = null;
    }
    #endregion
    private bool _didInit;
    private void LoadSpinnerBoardScores()
    {
        LoadScores();
        LoadSpinner();
    }
    private void LoadProperScreens()
    {
        switch (mainGame.SaveRoot.GameStatus)
        {
            case EnumWhatStatus.NeedChooseFirstOption:
            case EnumWhatStatus.NeedNight:
            case EnumWhatStatus.NeedToChooseSpace:
            case EnumWhatStatus.NeedChooseRetirement:
            case EnumWhatStatus.NeedToEndTurn:
                LoadScores();
                break;
            case EnumWhatStatus.None:
                {
                    throw new CustomBasicException("The status cannot be done.  Rethink");
                }
            case EnumWhatStatus.NeedToSpin:
            case EnumWhatStatus.LastSpin:
            case EnumWhatStatus.MakingMove:
                LoadSpinnerBoardScores();
                break;
            case EnumWhatStatus.NeedChooseStock:
                LoadChooseStock();
                break;
            case EnumWhatStatus.NeedChooseFirstCareer:
            case EnumWhatStatus.NeedNewCareer:
                LoadCareer();
                break;
            case EnumWhatStatus.NeedChooseSalary:
                LoadChooseSalary();
                break;
            case EnumWhatStatus.NeedChooseGender:
                throw new CustomBasicException("Cannot choose gender on main view model.  Rethink");
            case EnumWhatStatus.NeedChooseHouse:
                LoadSpinner();
                LoadHouse();
                break;
            case EnumWhatStatus.NeedTradeSalary:
                LoadTradeSalary();
                LoadScores();
                break;
            case EnumWhatStatus.NeedStealTile:
                LoadScores();
                LoadStealTiles();
                break;
            case EnumWhatStatus.NeedReturnStock:
                LoadReturnStock();
                LoadScores();
                break;
            case EnumWhatStatus.NeedSellBuyHouse:
            case EnumWhatStatus.NeedSellHouse:
            case EnumWhatStatus.NeedFindSellPrice:
                LoadSpinner();
                LoadScores();
                LoadShowCard();
                break;
            default:
                break;
        }
    }
    protected override Task OpenAsync()
    {
        LoadProperScreens();
        _didInit = true;
        return base.OpenAsync();
    }
    private void ChangeScreens(EnumWhatStatus oldStatus, EnumWhatStatus newStatus)
    {
        if (oldStatus == EnumWhatStatus.NeedTradeSalary)
        {
            CloseTradeSalary();
        }
        if (oldStatus == EnumWhatStatus.NeedStealTile)
        {
            CloseStealTiles();
        }
        if (oldStatus == EnumWhatStatus.NeedChooseHouse || oldStatus == EnumWhatStatus.NeedSellBuyHouse || oldStatus == EnumWhatStatus.NeedSellHouse)
        {
            CloseHouse();
        }

        switch (newStatus)
        {
            case EnumWhatStatus.NeedChooseFirstOption:
            case EnumWhatStatus.NeedNight:
            case EnumWhatStatus.NeedToChooseSpace:
            case EnumWhatStatus.NeedChooseRetirement:
            case EnumWhatStatus.NeedToEndTurn:
                CloseSpinner();
                break;

            case EnumWhatStatus.NeedChooseSalary:
            case EnumWhatStatus.NeedChooseStock:
            case EnumWhatStatus.NeedChooseFirstCareer:
            case EnumWhatStatus.NeedNewCareer:
                CloseSpinner();
                CloseScore();
                break;
            case EnumWhatStatus.NeedSellBuyHouse:
                CloseScore();
                break;
            case EnumWhatStatus.NeedSellHouse:
            case EnumWhatStatus.NeedFindSellPrice:
                CloseHouse();
                CloseScore();
                break;
            case EnumWhatStatus.NeedTradeSalary:
            case EnumWhatStatus.NeedStealTile:
                CloseSpinner();
                break;
            default:
                break;
        }
        LoadProperScreens();
    }
    Task IHandleAsync<ShowCardEventModel>.HandleAsync(ShowCardEventModel message)
    {
        ShowCard(message.GameStatus);
        OnStateChanged();
        return Task.CompletedTask;
    }
    private EnumWhatStatus _gameStatus;
    void IHandle<EnumWhatStatus>.Handle(EnumWhatStatus message)
    {
        if (_didInit == false)
        {
            return;
        }
        if (message == _gameStatus)
        {
            return;
        }
        ChangeScreens(_gameStatus, message);
        OnStateChanged(); //try this.  at least fix part of the issues.
        _gameStatus = message;
    }
}