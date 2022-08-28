namespace SinisterSix.Core.Logic;
[SingletonGame]
public class SinisterSixMainGameClass
    : DiceGameClass<EightSidedDice, SinisterSixPlayerItem, SinisterSixSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly SinisterSixVMData _model;
    private bool _wasAuto;
    private readonly CommandContainer _command;
    private readonly IToast _toast;
    public SinisterSixMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        SinisterSixVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        SinisterSixGameContainer gameContainer,
        StandardRollProcesses<EightSidedDice, SinisterSixPlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _model = currentMod;
        roller.BeforeRollingAsync = BeforeRollingAsync;
        roller.CanRollAsync = CanRollAsync;
        _command = command;
        _toast = toast;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        _model.Cup!.HowManyDice = _model.Cup.DiceList.Count;
        SaveRoot!.LoadMod(_model!);
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
        SaveRoot!.ImmediatelyStartTurn = true;
        SaveRoot!.LoadMod(_model);
        SaveRoot.MaxRolls = 3;
        PlayerList!.ForEach(items => items.Score = 0);
        await FinishUpAsync(isBeginning);
    }

    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "removeselecteddice":
                await RemoveSelectedDiceAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        _model.Cup!.HowManyDice = 6;
        _wasAuto = false;
        _model.Cup.HideDice();
        _model.Cup.CanShowDice = false;
        ProtectedStartTurn();
        await ContinueTurnAsync();
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        if (SaveRoot!.RollNumber > SaveRoot.MaxRolls)
        {
            if (ContainsSix() == false)
            {
                _wasAuto = true;
                await EndTurnAsync();
                return;
            }
        }
        await ContinueTurnAsync();
    }
    private Task BeforeRollingAsync()
    {
        return Task.CompletedTask; //we have nothing else todo.  some times we do.  did not want to have to creae another interface just for that part.
    }
    private async Task<bool> CanRollAsync()
    {
        await Task.Delay(0);
        if (SaveRoot!.RollNumber > 1)
        {
            if (ContainsSix() == true)
            {
                _toast.ShowUserErrorToast("Must remove any dice that equals 6");
                return false;
            }
        }
        return true;
    }
    public async Task RemoveSelectedDiceAsync()
    {
        if (SingleInfo!.CanSendMessage(BasicData!) == true)
        {
            await Network!.SendAllAsync("removeselecteddice");
        }
        _model.Cup!.RemoveSelectedDice();
        if (SaveRoot!.RollNumber > SaveRoot.MaxRolls && ContainsSix() == false)
        {
            _wasAuto = true;
            await EndTurnAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    private bool ContainsSix()
    {
        var thisList = _model.Cup!.DiceList.ToBasicList();
        thisList.KeepConditionalItems(xx => xx.Value == 6);
        var temps = thisList.GetAllPossibleCombinations();
        return temps.Any(xx => xx.Sum(xx => xx.Value) == 6);
    }
    private async Task GameOverAsync()
    {
        SingleInfo = PlayerList.OrderBy(items => items.Score).Take(1).Single();
        await ShowWinAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo!.Score = _model.Cup!.DiceList.Sum(xx => xx.Value);
        _command.UpdateAll();
        if (_wasAuto == true && Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(1); //if auto is done, needs to see what happened.
        }
        if (WhoTurn == WhoStarts)
        {
            SaveRoot!.MaxRolls = SaveRoot.RollNumber - 1;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(); //i think
        if (WhoTurn == WhoStarts)
        {
            await GameOverAsync();
            return;
        }
        await StartNewTurnAsync();
    }
}