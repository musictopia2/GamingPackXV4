namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
public abstract class BasicGameClass<P, S> :
    IGameSetUp<P, S>
    , ICommonMultiplayer<P, S>
    , IEndTurn, IEndTurnNM
    where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>, new()
{
    public BasicGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        IViewModelData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        BasicGameContainer<P, S> gameContainer,
        ISystemError error,
        IToast toast
        )
    {
        MainContainer = mainContainer;
        Aggregator = aggregator;
        BasicData = basicData;
        Test = test;
        _currentMod = currentMod;
        State = state;
        Delay = delay;
        _command = command;
        _gameContainer = gameContainer;
        SaveRoot = mainContainer.Resolve<S>(); //i think this would be fine (?)
        PlayerList = SaveRoot.PlayerList;
        gameContainer.EndTurnAsync = EndTurnAsync;
        gameContainer.ContinueTurnAsync = ContinueTurnAsync;
        gameContainer.ShowWinAsync = ShowWinAsync;
        gameContainer.StartNewTurnAsync = StartNewTurnAsync;
        gameContainer.SaveStateAsync = SaveStateAsync;
        Network = gameContainer.Network;
        BasicData.DoFullScreen?.Invoke();
        Toast = toast;
        Error = error;
    }
    private bool _computerEndsTurn = false;
    bool IGameSetUp<P, S>.ComputerEndsTurn
    {
        set
        {
            _computerEndsTurn = value;
        }
    }
    private readonly IViewModelData _currentMod;
    IViewModelData IBasicGameProcesses<P>.CurrentMod => _currentMod;
    public IEventAggregator Aggregator { get; set; }
    public IGamePackageResolver MainContainer { get; set; }
    public IAsyncDelayer? Delay;
    private readonly CommandContainer _command;
    private readonly BasicGameContainer<P, S> _gameContainer;
    public S SaveRoot
    {
        get => _gameContainer.SaveRoot!;
        set => _gameContainer.SaveRoot = value;
    }
    protected bool IsLoaded; //so when loaded, then can act differently.
    public PlayerCollection<P> PlayerList { get; set; }
    protected IMultiplayerSaveState State;
    //games like three letter fun needed to know the information about it.
    public BasicData BasicData { private set; get; } //this way others can access this data.
    public IGameNetwork? Network { get; set; } //has to be completely open now.
    public TestOptions? Test { private set; get; }
    public P? SingleInfo
    {
        get => _gameContainer.SingleInfo;
        set => _gameContainer.SingleInfo = value;
    }
    public int WhoTurn
    {
        get => SaveRoot.PlayOrder.WhoTurn;
        set => SaveRoot.PlayOrder.WhoTurn = value;
    }
    protected int WhoStarts
    {
        get => SaveRoot.PlayOrder.WhoStarts;
        set => SaveRoot.PlayOrder.WhoStarts = value;
    }
    public virtual Task PopulateSaveRootAsync()
    {
        return Task.CompletedTask;
    }
    public virtual bool CanMakeMainOptionsVisibleAtBeginning => true; //so others can decide not to after all.
    public Func<bool, Task>? FinishUpAsync { get; set; }
    public ISystemError Error { get; }
    public IToast Toast { get; }
    /// <summary>
    /// this is after getting saved data.
    /// </summary>
    /// <returns></returns>
    public abstract Task FinishGetSavedAsync();
    public virtual Task EndTurnAsync()
    {
        return Task.CompletedTask; //its up to each implementation to decide what to do.
    }
    /// <summary>
    /// you need to remember to have the loader finish up.  however, you can decide to call privatefinishasync as well.
    /// </summary>
    /// <param name="IsBeginning"></param>
    /// <returns></returns>
    public abstract Task SetUpGameAsync(bool isBeginning);
    protected async Task PrivateFinishAsync(bool isBeginning)
    {
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The finish up was not hooked up.  Rethink");
        }
        await FinishUpAsync.Invoke(isBeginning);
    }
    protected virtual Task ComputerTurnAsync()
    {
        return Task.CompletedTask; //if you do nothing, will be stuck.
    }
    protected virtual void GetPlayerToContinueTurn()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
    }
    protected virtual Task LoadPossibleOtherScreensAsync() { return Task.CompletedTask; } //most of the time nothing but we reserve the option to do something if necessary including async.
    protected virtual async Task ShowHumanCanPlayAsync() //reserve the right to be async.
    {
        InProgressHelpers.MoveInProgress = false;
        _command.IsExecuting = false;
        _command.ManuelFinish = false;
        _command.Processing = false;
        _command.ManualReport();
        await Task.CompletedTask;
    }
    public async Task SaveStateAsync()
    {
        if (BasicData!.MultiPlayer == true && BasicData.Client == true)
        {
            return;
        }
        await PopulateSaveRootAsync();
        await State!.SaveStateAsync(SaveRoot!);
    }
    public virtual async Task ShowWinAsync() //because board games has to also do something else.
    {
        this.ProtectedShowWin();
        await this.ProtectedGameOverNextAsync();
    }
    protected async Task ShowWinAsync(string customMessage) //this is needed for games like millebournes.
    {
        this.ProtectedShowCustomWin(customMessage);
        await this.ProtectedGameOverNextAsync();
    }
    public virtual async Task ShowTieAsync() //because board games has to also do something else.
    {
        this.ProtectedShowTie();
        await this.ProtectedGameOverNextAsync();
    }
    protected async Task ShowLossAsync() //for games like old maid.
    {
        this.ProtectedShowLoss();
        await this.ProtectedGameOverNextAsync();
    }
    public async virtual Task ContinueTurnAsync() //we do open the possibility of overriding if necessary.
    {
        InProgressHelpers.MoveInProgress = true;
        await SaveStateAsync();
        GetPlayerToContinueTurn();
        await LoadPossibleOtherScreensAsync();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            await ShowHumanCanPlayAsync();
            return;
        }
        _command.ManuelFinish = true;
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
        {
            if (BasicData!.MultiPlayer == false || BasicData.Client == false)
            {
                if (Test!.ComputerEndsTurn == true && BasicData.MultiPlayer == true)
                {
                    throw new CustomBasicException("If the computer player is going to skip their turns and its multiplayer, then why did you add extra computer players?");
                }
                if (_computerEndsTurn)
                {
                    _command.Processing = false;
                    if (Test.NoComputerPause == false)
                    {
                        await Delay!.DelayMilli(500); //i think you should see it showed computer player though.
                    }
                    await EndTurnAsync();
                    return;
                }
                _command.UpdateAll();
                await ComputerTurnAsync();
                return;
            }
        }
        if (BasicData!.MultiPlayer == false)
        {
            throw new CustomBasicException("Stuck because not multiplayer.  Rethink");
        }
        if (InProgressHelpers.Reconnecting == false)
        {
            Network!.IsEnabled = true;
        }
        else
        {
            Network!.IsEnabled = false;
        }
        _command.IsExecuting = true;
        InProgressHelpers.MoveInProgress = false;
        _command.UpdateAll();
    }
    public virtual async Task EndTurnReceivedAsync(string data)
    {
        await EndTurnAsync();
    }
    protected virtual void PrepStartTurn() //board games has to do something else in addition.  so upon autoresume, it can still if color is not chosen to show colors to choose.
    {
        SaveRoot!.ImmediatelyStartTurn = false;
        SingleInfo = PlayerList!.GetWhoPlayer();
        this.ShowTurn();
    }
    public abstract Task StartNewTurnAsync(); //decided to make it abstract this time.
}