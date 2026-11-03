namespace Payday.Core.ViewModels;
[InstanceGame]
[EventAggregatorFromMemberClass(nameof(Aggregator))]
[UseStateChanged]
public partial class PaydayMainViewModel(CommandContainer commandContainer,
        PaydayMainGameClass mainGame,
        PaydayVMData model,
        BasicData basicData,
        IGamePackageResolver resolver,
        IBuyProcesses processes,
        IEventAggregator aggregator) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator), IHandle<EnumStatus>
{
    public PaydayVMData VMData { get; set; } = model;
    private partial void Subscribe();
    private partial void Unsubscribe();
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    private bool _didInit;
    protected override void AdditionalInitialize()
    {
        Subscribe();
        base.AdditionalInitialize();
    }
    protected override async Task CloseAsync()
    {
        Unsubscribe();
        CloseBuyDealScreen();
        CloseChooseDealScreen();
        CloseDealOrBuyScreen();
        CloseDealPileScreen();
        CloseLotteryScreen();
        CloseMailListScreen();
        CloseMailPileScreen();
        ClosePlayerScreen();
        CloseRollerScreen();
        await base.CloseAsync();
    }
    protected override Task OpenAsync()
    {
        VMData.DealPile.SendEnableProcesses(this, () => mainGame.SaveRoot.GameStatus == EnumStatus.ChooseBuy);
        VMData.CurrentDealList.ObjectClickedAsync = CurrentDealList_ObjectClickedAsync;
        LoadProperScreens();
        return base.OpenAsync();
    }

    private async Task CurrentDealList_ObjectClickedAsync(DealCard payLoad, int index)
    {
        if (VMData.GameStatus != EnumStatus.ChooseBuy)
        {
            return; //because you can't do it.
        }
        await processes.BuyerSelectedAsync(payLoad.Deck);
    }
    private void LoadProperScreens()
    {
        switch (mainGame.SaveRoot.GameStatus)
        {
            case EnumStatus.Starts:
            case EnumStatus.None:
            case EnumStatus.MakeMove:
            case EnumStatus.RollLottery:
            case EnumStatus.RollRadio:
            case EnumStatus.EndingTurn:
            case EnumStatus.RollCharity:
                LoadMainScreens();
                break;
            case EnumStatus.ChooseDeal:
                CloseRollerScreen();
                CloseMailListScreen();
                LoadDealPileScreen();
                LoadChooseDealScreen();
                break;
            case EnumStatus.ChoosePlayer:
                CloseMailListScreen();
                CloseRollerScreen();
                LoadMailPileScreen();
                LoadPlayerScreen();
                break;
            case EnumStatus.ChooseLottery:
                CloseMailListScreen();
                CloseRollerScreen();
                LoadLotteryScreen();
                break;
            case EnumStatus.ChooseBuy:
                //leave the comments because not sure about other platforms (?)
                LoadMainScreens(); //for now, does not know anything about xamarin forms
                                   //may require rethinking (?)

                //if (mainGame.BasicData.IsXamarinForms)
                //{
                //    CloseMailListScreen();
                //    CloseDealOrBuyScreen();
                //    LoadBuyDealScreen();
                //}
                //else
                //{
                //    LoadMainScreens(); //desktop can show all.
                //}
                break;
            case EnumStatus.DealOrBuy:
                CloseRollerScreen();
                LoadMailPileScreen();
                CloseMailListScreen();
                LoadDealOrBuyScreen();
                break;
            case EnumStatus.ViewMail:
                CloseRollerScreen();
                LoadMailPileScreen();
                break;
            case EnumStatus.ViewYardSale:
                CloseRollerScreen();
                LoadDealPileScreen();
                break;
            default:
                break;
        }
        _didInit = true;
    }
    #region Screen Processes
    private void CloseBuyDealScreen()
    {
        if (BuyDealScreen == null)
        {
            return;
        }
        BuyDealScreen = null;
    }
    private void CloseChooseDealScreen()
    {
        if (ChooseDealScreen == null)
        {
            return;
        }
        ChooseDealScreen = null;
    }
    private void CloseDealOrBuyScreen()
    {
        if (DealOrBuyScreen == null)
        {
            return;
        }
        DealOrBuyScreen = null;
    }
    private void CloseDealPileScreen()
    {
        if (ShowDealPile == false)
        {
            return;
        }
        ShowDealPile = false;
    }
    private void CloseLotteryScreen()
    {
        if (LotteryScreen == null)
        {
            return;
        }
        LotteryScreen = null;
    }
    private void CloseMailPileScreen()
    {
        if (ShowMailPile == false)
        {
            return;
        }
        ShowMailPile = false;
    }
    private void ClosePlayerScreen()
    {
        if (PlayerScreen == null)
        {
            return;
        }
        PlayerScreen = null;
    }
    private void CloseRollerScreen()
    {
        if (RollerScreen == null)
        {
            return;
        }
        RollerScreen = null;
    }
    private void CloseMailListScreen()
    {
        if (ShowMailList == false)
        {
            return;
        }
        ShowMailList = false;
    }
    [StateProperty]
    public partial bool ShowMailList { get; private set; }
    [StateProperty]
    public partial bool ShowDealPile { get; private set; }
    [StateProperty]
    public partial bool ShowMailPile { get; private set; }
    [StateProperty]
    public partial BuyDealViewModel? BuyDealScreen { get; set; }
    [StateProperty]
    public partial ChooseDealViewModel? ChooseDealScreen { get; set; }
    [StateProperty]
    public partial DealOrBuyViewModel? DealOrBuyScreen { get; set; }
    [StateProperty]
    public partial LotteryViewModel? LotteryScreen { get; set; }
    [StateProperty]
    public partial PlayerPickerViewModel? PlayerScreen { get; set; }
    [StateProperty]
    public partial RollerViewModel? RollerScreen { get; set; }
    private void LoadMailListScreen()
    {
        if (ShowMailList)
        {
            return;
        }
        ShowMailList = true;
    }
    private void LoadChooseDealScreen()
    {
        if (ChooseDealScreen != null)
        {
            return;
        }
        ChooseDealScreen = Resolver.Resolve<ChooseDealViewModel>();
    }
    private void LoadDealOrBuyScreen()
    {
        if (DealOrBuyScreen != null)
        {
            return;
        }
        DealOrBuyScreen = Resolver.Resolve<DealOrBuyViewModel>();
    }
    private void LoadDealPileScreen()
    {
        if (ShowDealPile)
        {
            return;
        }
        ShowDealPile = true;
    }
    private void LoadLotteryScreen()
    {
        if (LotteryScreen != null)
        {
            return;
        }
        LotteryScreen = Resolver.Resolve<LotteryViewModel>();
    }
    private void LoadMailPileScreen()
    {
        if (ShowMailPile)
        {
            return;
        }
        ShowMailPile = true;
    }
    private void LoadPlayerScreen()
    {
        if (PlayerScreen != null)
        {
            return;
        }
        PlayerScreen = Resolver.Resolve<PlayerPickerViewModel>();
    }
    private void LoadRollerScreen()
    {
        if (RollerScreen != null)
        {
            return;
        }
        RollerScreen = Resolver.Resolve<RollerViewModel>();
    }
    private void LoadMainScreens()
    {
        CloseBuyDealScreen();
        CloseChooseDealScreen();
        CloseDealOrBuyScreen();
        CloseDealPileScreen();
        CloseLotteryScreen();
        CloseMailPileScreen();
        ClosePlayerScreen();
        LoadRollerScreen();
        LoadMailListScreen();
    }
    private EnumStatus _gameStatus;
    void IHandle<EnumStatus>.Handle(EnumStatus message)
    {
        if (_didInit == false)
        {
            return;
        }
        if (message == _gameStatus)
        {
            return;
        }
        _gameStatus = message;
        LoadProperScreens();
    }
    #endregion
}