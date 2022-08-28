namespace YachtRace.Core.Logic;
[SingletonGame]
public class YachtRaceMainGameClass
    : DiceGameClass<SimpleDice, YachtRacePlayerItem, YachtRaceSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly YachtRaceVMData _model;
    private readonly CommandContainer _command;
    internal bool HasRolled { get; set; }
    public YachtRaceMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        YachtRaceVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        YachtRaceGameContainer gameContainer,
        StandardRollProcesses<SimpleDice, YachtRacePlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _model = currentMod;
        _command = command;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        LoadMod();
        IsLoaded = true;
    }
    private void PrepTurn()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        ProtectedStartTurn(); //i think.
        HasRolled = false;
        this.ShowTurn();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.Stops.Reset();
        }
    }
    protected override async Task ComputerTurnAsync()
    {
        await Task.CompletedTask;
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SetUpDice();
        PlayerList!.ForEach(thisPlayer => thisPlayer.Time = 0);
        PrepTurn();
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "fivekind":
                await ProcessFiveKindAsync(float.Parse(content));
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        PrepTurn();
        await ContinueTurnAsync();
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        HasRolled = true;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        _model.Cup!.UnholdDice();
        _command.ManuelFinish = true;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        if (WhoTurn == WhoStarts)
        {
            SingleInfo = PlayerList.OrderBy(items => items.Time).First();
            await ShowWinAsync();
            return;
        }
        await StartNewTurnAsync();
    }
    internal async Task ProcessFiveKindAsync(float howLong)
    {
        SingleInfo!.Time = howLong;
        HasRolled = false;
        await EndTurnAsync();
    }
    internal bool HasYahtzee()
    {
        if (Test!.AllowAnyMove)
        {
            return true;
        }
        var count = Cup!.DiceList.DistinctCount(items => items.Value);
        return count == 1; //hopefully this works.
    }
}