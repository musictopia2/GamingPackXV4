namespace BuncoDiceGame.Core.ViewModels;
[InstanceGame]
public partial class BuncoDiceGameMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer,
    IHandleAsync<ChoseNewRoundEventModel>
{
    private readonly IGamePackageResolver _resolver;
    private readonly ISaveSinglePlayerClass _state;
    private readonly GlobalClass _global;
    private readonly BasicData _basicData;
    private readonly BuncoDiceGameMainGameClass _mainGame;
    private readonly IToast _toast;
    private readonly IMessageBox _message;
    public DiceCup<SimpleDice>? ThisCup;
    #region Properties
    public bool CanEndTurn { get; set; }
    public bool AlreadyReceivedBunco { get; set; }
    #endregion
    public BuncoDiceGameMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        IToast toast,
        IMessageBox message,
        ISaveSinglePlayerClass state,
            GlobalClass global,
            BasicData basicData
        ) : base(aggregator)
    {
        _toast = toast;
        _message = message;
        CommandContainer = commandContainer;
        _resolver = resolver;
        _state = state;
        _global = global;
        _basicData = basicData;
        CommandContainer.ManuelFinish = true;
        CommandContainer.IsExecuting = true; //not sure.
        _mainGame = resolver.ReplaceObject<BuncoDiceGameMainGameClass>(); //hopefully this works.  means you have to really rethink.
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    public CommandContainer CommandContainer { get; set; }
    public bool CanEnableBasics()
    {
        return _global.IsActive; //because maybe you can't enable it.
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = true;
        await base.ActivateAsync();
        _global.IsActive = true;
        await _mainGame.StartGameAsync(GetLoadedCup);
        _basicData.GameDataLoading = false;
    }
    private BuncoDiceGameSaveInfo? _saveroot;
    private DiceCup<SimpleDice> GetLoadedCup(BuncoDiceGameSaveInfo saveRoot, bool autoResume)
    {
        _saveroot = saveRoot; //hopefully this simple (?)
        if (_saveroot!.MaxedRolls)
        {
            CanEndTurn = true;
        }
        if (ThisCup != null)
        {
            return ThisCup;
        }
        ThisCup = new(saveRoot.DiceList, _resolver, CommandContainer)
        {
            HowManyDice = 3,
            ShowDiceListAlways = true
        };
        if (autoResume)
        {
            ThisCup.ClearDice();
        }
        else
        {
            ThisCup.CanShowDice = true;
        }
        return ThisCup;
    }
    Task IHandleAsync<ChoseNewRoundEventModel>.HandleAsync(ChoseNewRoundEventModel message)
    {
        return _mainGame.ProcessNewRoundAsync(); //hopefully this simple (?)
    }
    public bool CanRoll => !CanEndTurn;
    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        await _mainGame.RollDiceAsync();
        int score = _mainGame.ScoreRoll();
        if (score == 0)
        {
            CanEndTurn = true; //could be iffy.
            _saveroot!.MaxedRolls = true;
            await _state.SaveSimpleSinglePlayerGameAsync(_saveroot); //needs to save.  this could have been a serious bug.
            return;
        }
        _mainGame.UpdateScores(score);
        _saveroot!.HasRolled = true;
        await _state.SaveSimpleSinglePlayerGameAsync(_saveroot); //needs to save.  this could have been a serious bug.
    }
    public bool CanBunco()
    {
        if (CanEndTurn)
        {
            return false;
        }
        if (AlreadyReceivedBunco)
        {
            return false;
        }
        return _saveroot!.HasRolled;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task BuncoAsync()
    {
        CommandContainer.ManuelFinish = false;
        if (_mainGame.DidHaveBunco() == false)
        {
            await _message.ShowMessageAsync("Sorry, there is no bunco here");
            return;
        }
        _mainGame.UpdateScores(16);
        _mainGame.ReceivedBunco();
        _saveroot!.ThisStats!.YourPoints = _mainGame.CurrentPlayer!.Points;
        _saveroot.ThisStats.Buncos = _mainGame.CurrentPlayer.Buncos;
        if (_mainGame.CurrentPlayer.Table == 1)
        {
            await _mainGame.FinishRoundAsync(); // because a bunco has been received and you are hosting.  if you are not hosting, then round does not end right away.
            return;
        }
        AlreadyReceivedBunco = true;
    }
    public bool CanHuman21()
    {
        //return false;
        if (CanEndTurn == false)
        {
            return false;
        }
        if (_saveroot!.HasRolled == false)
        {
            return false;
        }
        return _mainGame.CurrentPlayer!.Table == 1;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task Human21Async()
    {
        CommandContainer.ManuelFinish = false;
        if (_mainGame!.CurrentPlayer!.Points < 21)
        {
            _toast.ShowUserErrorToast("Sorry, you do not have 21 points.  Therefore, yuo cannot end the round");
            return;
        }
        if (_mainGame.CurrentPlayer.Table > 1)
        {
            _toast.ShowUserErrorToast("Sorry, you cannot end this round because you are not hosting this round.");
            return;
        }
        await _mainGame.FinishRoundAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task EndTurnAsync()
    {
        CanEndTurn = false;
        AlreadyReceivedBunco = false;
        await _mainGame.EndTurnAsync();
    }
}