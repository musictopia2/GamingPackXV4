namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;

public abstract class DiceGameClass<D, P, S> : BasicGameClass<P, S>,
    IStandardRoller<D, P>,
    IProcessHoldNM,
    IDiceMainProcesses<P>,
    IHoldUnholdProcesses,
    IBeginningDice<D, P, S>

    where D : IStandardDice, new()
     where P : class, IPlayerItem, new()
    where S : BasicSavedDiceClass<D, P>, new()
{
    private readonly IBasicDiceGamesData<D> _model;
    public DiceGameClass(
        IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        IBasicDiceGamesData<D> currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        BasicGameContainer<P, S> gameContainer,
        StandardRollProcesses<D, P> roller,
        ISystemError error,
        IToast toast
        ) : base(
            mainContainer,
            aggregator,
            basicData,
            test,
            currentMod,
            state,
            delay,
            command,
            gameContainer,
            error,
            toast)
    {
        _model = currentMod;
        Roller = roller;
        Roller.CurrentPlayer = () => SingleInfo!;
        Roller.AfterRollingAsync = AfterRollingAsync;
        Roller.AfterSelectUnselectDiceAsync = AfterSelectUnselectDiceAsync;
    }
    public DiceCup<D>? Cup => _model.Cup;
    protected virtual bool ShowDiceUponAutoSave => true;
    protected void LoadMod()
    {
        SaveRoot.LoadMod(_model);
    }
    protected void SetUpDice()
    {
        _model.LoadCup(SaveRoot!, false);
        SaveRoot!.RollNumber = 1;
        SaveRoot.DiceList.MainContainer = MainContainer;
    }
    protected void ProtectedStartTurn()
    {
        SaveRoot!.RollNumber = 1;
    }
    protected StandardRollProcesses<D, P> Roller { get; } //the computer may need it.
    protected void AfterRestoreDice()
    {
        _model.LoadCup(SaveRoot!, true);
        SaveRoot!.DiceList.MainContainer = MainContainer;
        if (ShowDiceUponAutoSave == true)
        {
            _model.Cup!.CanShowDice = true;
            _model.Cup.ShowDiceListAlways = true;
            _model.Cup.Visible = true;
        }
        else
        {
            _model.Cup!.CanShowDice = false;
        }
        LoadMod();
    }
    protected async Task SendHoldMessageAsync(int index)
    {
        if (SingleInfo!.CanSendMessage(BasicData!) == true)
        {
            await Network!.SendAllAsync("processhold", index);
        }
    }
    public virtual async Task HoldUnholdDiceAsync(int index)
    {
        await SendHoldMessageAsync(index);
        Cup!.HoldUnholdDice(index);
        await AfterHoldUnholdDiceAsync();
    }
    public async Task ProcessHoldReceivedAsync(int id)
    {
        await HoldUnholdDiceAsync(id);
    }
    public virtual async Task AfterHoldUnholdDiceAsync()
    {
        await ContinueTurnAsync();
    }
    public async Task AfterRollingAsync()
    {
        SaveRoot!.RollNumber++;
        await ProtectedAfterRollingAsync();
    }
    abstract protected Task ProtectedAfterRollingAsync();
    public virtual async Task AfterSelectUnselectDiceAsync()
    {
        await ContinueTurnAsync();
    }
}