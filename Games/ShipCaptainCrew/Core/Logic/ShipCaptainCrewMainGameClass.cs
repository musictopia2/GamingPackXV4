namespace ShipCaptainCrew.Core.Logic;
[SingletonGame]
public class ShipCaptainCrewMainGameClass
    : DiceGameClass<SimpleDice, ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>
    , ISerializable
{
    private readonly ShipCaptainCrewVMData _model;
    private readonly CommandContainer _command;
    private readonly StandardRollProcesses<SimpleDice, ShipCaptainCrewPlayerItem> _roller;
    private readonly IToast _toast;
    public ShipCaptainCrewMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        ShipCaptainCrewVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        ShipCaptainCrewGameContainer gameContainer,
        StandardRollProcesses<SimpleDice, ShipCaptainCrewPlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _model = currentMod;
        _command = command;
        _roller = roller;
        _toast = toast;
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
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelayMilli(300);
        }
        await _roller.RollDiceAsync();
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
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.Wins = 0;
            thisPlayer.WentOut = false;
            thisPlayer.Score = 0;
            thisPlayer.TookTurn = false;
        });
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        _model!.Cup!.HideDice();
        SaveRoot.RollNumber = 1;
        _model.Cup.CanShowDice = false;
        SingleInfo!.TookTurn = false;
        _model.Cup.UnholdDice();
        await ContinueTurnAsync();
    }
    protected override bool ShowDiceUponAutoSave => SaveRoot!.RollNumber > 1;
    private void RefreshPlay()
    {
        PlayerList.ForEach(x => x.Score = 0);
        PlayerList.ForConditionalItems(x => x.InGame == true, (x => x.TookTurn = false));
    }
    private async Task LastStepAsync()
    {
        if (PlayerList.Count(items => items.InGame == true) == 2)
        {
            var tieList = PlayerList.OrderByDescending(items => items.Score).Take(2).ToBasicList();
            if (tieList.First().Score == tieList.Last().Score)
            {
                RefreshPlay();
                await StartNewTurnAsync();
                return;
            }
            int oldTurn = WhoTurn;
            WhoTurn = tieList.First().Id;
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.Wins++;
            if (SingleInfo.Wins == 2)
            {
                await ShowWinAsync();
                return;
            }
            RefreshPlay();
            WhoTurn = oldTurn;
            await StartNewTurnAsync();
            return;
        }
        var thisList = PlayerList.Where(items => items.InGame == true).OrderBy(items => items.Score).Take(2).ToBasicList();
        if (thisList.Count < 2)
        {
            throw new CustomBasicException("Must be at least 2 players");
        }
        if (thisList.First().Score == thisList.Last().Score)
        {
            await StartNewTurnAsync();
            return;
        }
        thisList.First().InGame = false;
        thisList.First().WentOut = true;
        thisList.First().TookTurn = true;
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.InGame == false)
        {
            WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
        }
        PlayerList.ForEach(thisPlayer => thisPlayer.Score = 0);
        await StartNewTurnAsync();
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        HoldShips();
        if (SaveRoot!.RollNumber <= 3 && _model.Cup!.HowManyHeldDice() < 5)
        {
            await ContinueTurnAsync();
            return;
        }
        var otherList = GetShipList();
        if (otherList.Count < 3)
        {
            await EndTurnAsync();
            return;
        }
        var tempList = GetScoringGuide(otherList);
        var score = tempList.Sum(items => items.Value);
        SingleInfo!.Score = score;
        _command.UpdateAll();
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(1);
        }
        await EndTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        _command.ManuelFinish = true;
        SingleInfo!.TookTurn = true;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        if (PlayerList.All(x => x.TookTurn))
        {
            await LastStepAsync();
            return;
        }
        await StartNewTurnAsync();
    }
    public override async Task HoldUnholdDiceAsync(int index)
    {
        var thisList = GetShipList();
        if (thisList.Any(items => items.Index == index))
        {
            _toast.ShowUserErrorToast("Cannot unselect a part of a ship");
            return;
        }
        if (thisList.Count < 3)
        {
            await base.HoldUnholdDiceAsync(index);
        }
        var scoreList = GetScoringGuide(thisList);
        if (scoreList.Any(items => items.Index == index) == false)
        {
            throw new CustomBasicException($"{index} was not found on the scoring or the ship.");
        }
        var thisDice = scoreList.First(items => items.Index == index);
        bool willHold = thisDice.Hold;
        scoreList.ForEach(items => items.Hold = willHold);
        await SendHoldMessageAsync(index);
        await ContinueTurnAsync();
    }
    public override Task AfterHoldUnholdDiceAsync()
    {
        return base.AfterHoldUnholdDiceAsync();
    }
    private void HoldShips()
    {
        var tempList = GetShipList();
        tempList.ForEach(thisDice => thisDice.Hold = true);
    }
    private BasicList<SimpleDice> GetShipList()
    {
        BasicList<SimpleDice> output = new();
        for (int x = 4; x <= 6; x++)
        {
            if (_model.Cup!.DiceList.Any(items => items.Value == x))
            {
                output.Add(_model.Cup.DiceList.First(items => items.Value == x));
            }
        }
        return output;
    }
    private BasicList<SimpleDice> GetScoringGuide(BasicList<SimpleDice> shipList)
    {
        var output = _model.Cup!.DiceList.ToBasicList();
        output.RemoveGivenList(shipList);
        if (output.Count > 2)
        {
            throw new CustomBasicException("Can only have a maximum of 2 items after removing the ship");
        }
        return output;
    }
}