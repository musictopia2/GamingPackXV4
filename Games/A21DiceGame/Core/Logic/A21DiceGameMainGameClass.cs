namespace A21DiceGame.Core.Logic;
[SingletonGame]
public class A21DiceGameMainGameClass
    : DiceGameClass<SimpleDice, A21DiceGamePlayerItem, A21DiceGameSaveInfo>
    , ISerializable
{
    private readonly A21DiceGameVMData? _model;
    private readonly StandardRollProcesses<SimpleDice, A21DiceGamePlayerItem> _roller;
    private readonly IToast _toast;
    public A21DiceGameMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        A21DiceGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        A21DiceGameGameContainer gameContainer,
        StandardRollProcesses<SimpleDice, A21DiceGamePlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _model = currentMod;
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
        IsLoaded = true; //i think needs to be here.
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelayMilli(300);
        }
        if (SingleInfo!.Score <= 15)
        {
            await _roller!.RollDiceAsync();
            return;
        }
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendEndTurnAsync();
        }
        await EndTurnAsync();
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        SetUpDice();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SaveRoot!.IsFaceOff = false;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.IsFaceOff = false;
            thisPlayer.Score = 0;
            thisPlayer.NumberOfRolls = 0;
        });
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn(); //anything else is below.
        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        SingleInfo!.Score += _model!.Cup!.DiceList.Sum(xx => xx.Value);
        if (SaveRoot!.IsFaceOff == true)
        {
            if (PlayerList.Any(xx => xx.IsFaceOff == true && xx.Score == 0))
            {
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                await StartNewTurnAsync();
                return;
            }
            await ExtendedFaceOffAsync();
            return;
        }
        SingleInfo.NumberOfRolls++;
        if (SingleInfo.Score > 21)
        {
            _toast.ShowInfoToast($"{SingleInfo.NickName} is out for going over 21");
            await EndTurnAsync();
            return;
        }
        await ContinueTurnAsync(); //if they get 21, they are responsible for ending turn.
    }
    public override async Task EndTurnAsync()
    {
        int oldTurn;
        oldTurn = WhoTurn;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        int howMany = PlayerList.Count(xx => xx.NumberOfRolls == 0);
        if (howMany == 1)
        {
            SingleInfo = PlayerList.GetWhoPlayer();
            var firstList = PlayerList.ToBasicList();
            firstList.RemoveSpecificItem(SingleInfo);
            if (firstList.Any(xx => xx.Score <= 21) == false)
            {
                await ShowWinAsync();
                return;
            }
        }
        else if (howMany == 0)
        {
            await StartEndAsync();
            return;
        }
        await StartNewTurnAsync();
    }
    #region "Unique Game Features"
    private async Task StartEndAsync()
    {
        var firstList = PlayerList.Where(Items => Items.Score <= 21).OrderByDescending(Items => Items.Score).ThenBy(Items => Items.NumberOfRolls).ToBasicList();
        if (firstList.Count == 1 || firstList.First().Score > firstList[1].Score)
        {
            SingleInfo = firstList.First();
            await ShowWinAsync();
            return;
        }
        if (firstList.First().Score == firstList[1].Score && firstList.First().NumberOfRolls < firstList[1].NumberOfRolls)
        {
            SingleInfo = firstList.First();
            await ShowWinAsync();
            return;
        }
        SaveRoot!.IsFaceOff = true;
        WhoTurn = firstList.First().Id;
        var newList = PlayerList.Where(xx => xx.Score != firstList.First().Score && xx.NumberOfRolls != firstList.First().NumberOfRolls).ToBasicList();
        newList.ForEach(items =>
        {
            items.Score = 0;
            items.InGame = false;
        });
        newList = PlayerList.Where(xx => xx.InGame == true).ToBasicList();
        newList.ForEach(Items => Items.Score = 0);
        await StartNewTurnAsync();
    }
    private async Task ExtendedFaceOffAsync()
    {
        var thisList = PlayerList.Where(xx => xx.IsFaceOff == true).OrderByDescending(Items => Items.Score).ToBasicList();
        if (thisList.Count < 2)
        {
            throw new CustomBasicException("Must have at least 2 players for faceoff");
        }
        if (thisList.First().Score > thisList[1].Score)
        {
            SingleInfo = thisList.First();
            await ShowWinAsync();
            return;
        }
        var newList = PlayerList.Where(xx => xx.IsFaceOff == true && xx.Score < thisList.First().Score).ToBasicList();
        newList.ForEach(items =>
        {
            items.IsFaceOff = false;
            items.InGame = false;
            items.Score = 0;
        });
        newList = PlayerList.Where(xx => xx.IsFaceOff == true).ToBasicList();
        newList.ForEach(Items => Items.Score = 0);
        WhoTurn = thisList.First().Id;
        await StartNewTurnAsync();
    }
    #endregion
}