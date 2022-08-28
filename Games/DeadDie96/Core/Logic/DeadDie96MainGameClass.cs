namespace DeadDie96.Core.Logic;
[SingletonGame]
public class DeadDie96MainGameClass
    : DiceGameClass<TenSidedDice, DeadDie96PlayerItem, DeadDie96SaveInfo>
    , ISerializable
{
    private readonly DeadDie96VMData? _model;
    private readonly StandardRollProcesses<TenSidedDice, DeadDie96PlayerItem> _roller;
    public DeadDie96MainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        DeadDie96VMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        DeadDie96GameContainer gameContainer,
        StandardRollProcesses<TenSidedDice, DeadDie96PlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _model = currentMod;
        _roller = roller;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        _model!.Cup!.HowManyDice = _model.Cup.DiceList.Count; //try this way.
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        LoadMod();
        IsLoaded = true; //i think needs to be here.
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelayMilli(200);
        }
        await _roller!.RollDiceAsync();
    }
    private async Task GameOverAsync()
    {
        SingleInfo = PlayerList.OrderBy(Items => Items.TotalScore).Take(1).Single();
        await ShowWinAsync();
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SetUpDice();
        PlayerList!.ForEach(x =>
        {
            x.TotalScore = 0;
            x.CurrentScore = 0;
        });
        SaveRoot!.ImmediatelyStartTurn = true;
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        _model!.Cup!.HowManyDice = 5;
        _model.Cup.HideDice();
        _model.Cup.CanShowDice = false;
        ProtectedStartTurn();
        await ContinueTurnAsync();
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        var thisList = _model!.Cup!.DiceList.ToBasicList();
        if (thisList.Any(x => x.Value == 6 || x.Value == 9))
        {
            SingleInfo!.CurrentScore = 0;
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelayMilli(500);
            }
            _model.Cup.RemoveConditionalDice(xx => xx.Value == 6 || xx.Value == 9);
            if (_model.Cup.DiceList.Count == 0 || Test.ImmediatelyEndGame)
            {
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync();
            return;
        }
        int totalScore = _model.Cup.DiceList.Sum(xx => xx.Value);
        SingleInfo!.CurrentScore = totalScore;
        SingleInfo.TotalScore += totalScore;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        if (WhoTurn == WhoStarts)
        {
            await GameOverAsync();
            return;
        }
        await StartNewTurnAsync();
    }
}