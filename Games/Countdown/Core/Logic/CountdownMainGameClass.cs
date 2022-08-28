namespace Countdown.Core.Logic;
[SingletonGame]
public class CountdownMainGameClass
    : DiceGameClass<CountdownDice, CountdownPlayerItem, CountdownSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly CountdownVMData _model;
    private readonly CommandContainer _command;
    private readonly CountdownGameContainer _gameContainer;
    private readonly StandardRollProcesses<CountdownDice, CountdownPlayerItem> _roller;
    private readonly IToast _toast;
    public CountdownMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        CountdownVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        CountdownGameContainer gameContainer,
        StandardRollProcesses<CountdownDice, CountdownPlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _model = currentMod;
        _command = command;
        gameContainer.MakeMoveAsync = PrivateChooseNumberAsync;
        gameContainer.GetNumberList = GetNewNumbers;
        CountdownVMData.CanChooseNumber = CanChooseNumber;
        _gameContainer = gameContainer;
        _roller = roller;
        _toast = toast;
    }
    private async Task PrivateChooseNumberAsync(SimpleNumber number)
    {
        if (BasicData.MultiPlayer)
        {
            await Network!.SendAllAsync("choosenumber", number.Value);
        }
        await ProcessChosenNumberAsync(number);
    }
    public static BasicList<SimpleNumber> GetNewNumbers()
    {
        var firstList = Enumerable.Range(1, 10).ToList();
        return (from xx in firstList
                select new SimpleNumber() { Used = false, Value = xx }).ToBasicList();
    }
    public bool CanChooseNumber(SimpleNumber thisNumber)
    {
        if (thisNumber.Used == true)
        {
            return false; //i think this may be a better solution.
        }
        return SaveRoot!.HintList.Any(items => items == thisNumber.Value);
    }
    public async Task ProcessChosenNumberAsync(SimpleNumber thisNumber)
    {
        _gameContainer.Command.ManuelFinish = true;
        thisNumber.IsSelected = true;
        CountdownVMData.ShowHints = false;
        Aggregator.RepaintBoard();
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(1);
        }
        thisNumber.Used = true;
        thisNumber.IsSelected = false;
        Aggregator.RepaintBoard();
        await EndTurnAsync();
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls(); //stop statements don't work with webassembly unfortunately.
        AfterRestoreDice();
        if (Test!.DoubleCheck == true)
        {
            var thisList = _model.Cup!.DiceList.OrderByDescending(Items => Items.Value);
            if (thisList.Count() != 2)
            {
                throw new CustomBasicException("There should have been only 2 dice");
            }
            SaveRoot!.HintList = GetPossibleValues(thisList.First().Value, thisList.Last().Value);
        } //okay here too.
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        LoadMod();
        SaveRoot!.LoadMod(_model);
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        var thisList = SingleInfo!.NumberList.Where(x => x.Used == false).ToBasicList();
        var newList = thisList.Where(x => CanChooseNumber(x) == true).ToBasicList();
        if (newList.Count == 0)
        {
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelayMilli(200); //so you can at least see the computer tried to take their turn.
            }
            if (BasicData!.MultiPlayer == true)
            {
                throw new CustomBasicException("Should not be multiplayer because only 2 players are allowed");
            }
            await EndTurnAsync();
            return;
        }
        await ProcessChosenNumberAsync(newList.GetRandomItem());
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        SaveRoot!.Round = 1;
        SaveRoot.ImmediatelyStartTurn = true;
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SetUpDice();
        if (isBeginning == false)
        {
            ResetPlayers();
        }
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            case "choosenumber":
                SimpleNumber thisNumber = SingleInfo!.NumberList.Single(x => x.Value == int.Parse(content));
                await ProcessChosenNumberAsync(thisNumber);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        if (BasicData.MultiPlayer == true)
        {
            if (SingleInfo!.CanSendMessage(BasicData) == false)
            {
                Network!.IsEnabled = true;
                return;
            }
        }
        await _roller!.RollDiceAsync();
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        CountdownVMData.ShowHints = false;
        int DiceValue = _model!.Cup!.DiceList.Sum(items => items.Value);
        if (DiceValue == 12)
        {
            _toast.ShowInfoToast($"{SingleInfo!.NickName}  has to start over again for rolling a 12");
            await Delay!.DelaySeconds(2);
            await StartOverAsync(SingleInfo);
            return;
        }
        if (DiceValue == 11)
        {
            CountdownPlayerItem thisPlayer;
            if (WhoTurn == 1)
            {
                thisPlayer = PlayerList![2];
            }
            else
            {
                thisPlayer = PlayerList![1];
            }
            _toast.ShowInfoToast($"{thisPlayer.NickName}  has to start over again for rolling a 11");
            await Delay!.DelaySeconds(2);
            await StartOverAsync(thisPlayer);
            return;
        }
        var thisList = _model.Cup.DiceList.OrderByDescending(items => items.Value);
        if (thisList.Count() != 2)
        {
            throw new CustomBasicException("There should have been only 2 dice");
        }
        SaveRoot!.HintList = GetPossibleValues(thisList.First().Value, thisList.Last().Value);
        await ContinueTurnAsync();
        Aggregator.RepaintBoard();
    }
    public override async Task EndTurnAsync()
    {
        _command.ManuelFinish = true;
        SaveRoot!.HintList.Clear();
        if (SingleInfo!.NumberList.Any(items => items.Used == false) == false || Test!.ImmediatelyEndGame == true)
        {
            await GameOverAsync(false);
            return;
        }
        if (SaveRoot.Round >= 30 && WhoTurn != WhoStarts)
        {
            //has to end no matter what.
            int firstCount;
            int secondCount;
            CountdownPlayerItem thisPlayer = PlayerList![1];
            firstCount = thisPlayer.NumberList.Count(xx => xx.Used == true);
            thisPlayer = PlayerList[2];
            secondCount = thisPlayer.NumberList.Count(xx => xx.Used == true);
            bool wasTie = false;
            if (firstCount > secondCount)
            {
                SingleInfo = PlayerList[1];
            }
            else if (secondCount > firstCount)
            {
                SingleInfo = PlayerList[2];
            }
            else
            {
                wasTie = true;
            }
            await GameOverAsync(wasTie);
            return;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        if (WhoStarts == WhoTurn)
        {
            SaveRoot.Round++;
        }
        await StartNewTurnAsync();
    }
    private async Task GameOverAsync(bool wasTie)
    {
        if (wasTie == false)
        {
            await ShowWinAsync();
        }
        else
        {
            await ShowTieAsync();
        }
    }
    private async Task StartOverAsync(CountdownPlayerItem thisPlayer)
    {
        thisPlayer.NumberList.ForEach(items =>
        {
            items.IsSelected = false;
            items.Used = false;
        });
        Aggregator.RepaintBoard();
        await EndTurnAsync();
    }
    private static BasicList<int> GetPossibleValues(int firstValue, int secondValue)
    {
        int addValue;
        int subtractValue;
        addValue = firstValue + secondValue;
        subtractValue = firstValue - secondValue;
        BasicList<int> thisList = new()
        {
            addValue
        };
        if (subtractValue > 0)
        {
            thisList.Add(subtractValue);
        }
        int multValue;
        multValue = firstValue * secondValue;
        if (multValue <= 10 && multValue != addValue && multValue != secondValue)
        {
            thisList.Add(multValue);
        }
        if (firstValue == secondValue && addValue != 1 && secondValue != 1 && multValue != 1)
        {
            thisList.Add(1);
        }
        if (firstValue == 1 && secondValue == 1)
        {
            thisList.Add(1);// this is for sure valid for several reasons.
        }
        if (firstValue == 6 && secondValue == 2)
        {
            thisList.Add(3);
        }
        if (firstValue == 6 && secondValue == 3)
        {
            thisList.Add(2);
        }
        return thisList;
    }
    private void ResetPlayers()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.NumberList.ForEach(items =>
            {
                items.Used = false;
                items.IsSelected = false;
            });
        });
    }
}