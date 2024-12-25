
namespace RummyDice.Core.Logic;
[SingletonGame]
public class RummyDiceMainGameClass : BasicGameClass<RummyDicePlayerItem, RummyDiceSaveInfo>
    , ICommonMultiplayer<RummyDicePlayerItem, RummyDiceSaveInfo>
    , IMiscDataNM, ISerializable
{
    public RummyDiceMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        RummyDiceVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        RummyBoardCP rummyBoard,
        RummyDiceGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _test = test;
        _model = model;
        _command = command;
        MainBoard1 = rummyBoard;
        MainBoard1.SaveRoot = GetSave;
        MainBoard1.SelectOneMainAsync = SelectOneMainAsync;
    }
    private RummyDiceSaveInfo GetSave() => SaveRoot;
    private readonly TestOptions _test;
    private readonly RummyDiceVMData _model;
    private readonly CommandContainer _command;
    public RummyProcesses<EnumColorType, EnumColorType, RummyDiceInfo>? Rummys;
    public RummyBoardCP MainBoard1;
    public BasicList<PhaseList>? PhaseInfo;
    public BasicList<RummyDiceHandVM>? TempSets;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        if (SaveRoot.TempSets.Count != 2)
        {
            throw new CustomBasicException("You always have to have data for 2 temp sets even if you never put data there.");
        }
        int x = 0;
        SaveRoot.TempSets.ForEach(firsts =>
        {
            var thisItem = TempSets![x];
            thisItem.HandList = firsts.ToBasicList();
            x++;
        });
        PrepStartTurn();
        return Task.CompletedTask;
    }
    protected override void PrepStartTurn()
    {
        base.PrepStartTurn();
        _model.CurrentPhase = PhaseInfo![SingleInfo!.Phase - 1].Description;
    }
    public override Task PopulateSaveRootAsync()
    {
        int x = 0;
        SaveRoot!.TempSets.ForEach(firsts =>
        {
            firsts.Clear();
            var thisItem = TempSets![x];
            firsts.AddRange(thisItem.HandList);
            x++;
        });
        return base.PopulateSaveRootAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        TempSets = [];
        bool needsTemps;
        if (SaveRoot!.TempSets.Count == 0)
        {
            needsTemps = true;
        }
        else
        {
            needsTemps = false;
        }
        2.Times(x =>
        {
            RummyDiceHandVM thisTemp = new(_command, this);
            thisTemp.Index = x;
            TempSets.Add(thisTemp);
            if (needsTemps == true)
            {
                SaveRoot.TempSets.Add([]);
            }
        });
        CreateSets();
        IsLoaded = true;
    }
    private void CreateSets()
    {
        Rummys = new();
        Rummys.HasSecond = false;
        Rummys.HasWild = true;
        Rummys.LowNumber = 1;
        Rummys.HighNumber = 12;
        Rummys.NeedMatch = false;
        PhaseList thisPhase = new();
        thisPhase.Description = "2 Sets of 3";
        SetInfo newSets;
        PhaseInfo = [];
        2.Times(x =>
        {
            newSets = new();
            newSets.HowMany = 3;
            newSets.SetType = EnumWhatSets.Kinds;
            thisPhase.PhaseSets.Add(newSets);
        });
        PhaseInfo.Add(thisPhase);
        thisPhase = new();
        thisPhase.Description = "1 Set of 3, 1 Run of 4";
        2.Times(x =>
        {
            newSets = new();
            if (x == 2)
            {
                newSets.HowMany = 3;
                newSets.SetType = EnumWhatSets.Kinds;
            }
            else
            {
                newSets.HowMany = 4;
                newSets.SetType = EnumWhatSets.Runs;
            }
            thisPhase.PhaseSets.Add(newSets);
        });
        PhaseInfo.Add(thisPhase);
        thisPhase = new();
        thisPhase.Description = "1 Set of 4, 1 Run of 4";
        2.Times(x =>
        {
            newSets = new();
            newSets.HowMany = 4;
            if (x == 2)
            {
                newSets.SetType = EnumWhatSets.Kinds;
            }
            else
            {
                newSets.SetType = EnumWhatSets.Runs;
            }
            thisPhase.PhaseSets.Add(newSets);
        });
        PhaseInfo.Add(thisPhase);
        for (int x = 7; x <= 9; x++)
        {
            thisPhase = new();
            newSets = new();
            thisPhase.Description = "1 Run of " + x;
            newSets.SetType = EnumWhatSets.Runs;
            newSets.HowMany = x;
            thisPhase.PhaseSets.Add(newSets);
            PhaseInfo.Add(thisPhase);
        }
        thisPhase = new();
        thisPhase.Description = "2 Sets Of 4";
        for (int x = 1; x <= 2; x++)
        {
            newSets = new();
            newSets.HowMany = 4;
            newSets.SetType = EnumWhatSets.Kinds;
            thisPhase.PhaseSets.Add(newSets);
        }
        PhaseInfo.Add(thisPhase);
        thisPhase = new();
        thisPhase.Description = "7 Cards Of 1 Color";
        newSets = new();
        newSets.HowMany = 7;
        newSets.SetType = EnumWhatSets.Colors;
        thisPhase.PhaseSets.Add(newSets);
        PhaseInfo.Add(thisPhase);
        int Y;
        for (int x = 2; x <= 3; x++)
        {
            thisPhase = new();
            thisPhase.Description = "1 Set Of 5, 1 Set Of " + x;
            for (Y = 1; Y <= 2; Y++)
            {
                newSets = new();
                newSets.SetType = EnumWhatSets.Kinds;
                if (Y == 2)
                {
                    newSets.HowMany = x;
                }
                else
                {
                    newSets.HowMany = 5;
                }
                thisPhase.PhaseSets.Add(newSets);
            }
            PhaseInfo.Add(thisPhase);
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
        SaveRoot.SomeoneFinished = false;
        SaveRoot.DiceList.Clear();
        SaveRoot.RollNumber = 1;
        TempSets!.ForEach(thisSet =>
        {
            thisSet.HandList.Clear();
        });
        PlayerList!.ForEach(player =>
        {
            player.HowManyRepeats = 0;
            player.Phase = 1;
            player.ScoreGame = 0;
            player.ScoreRound = 0;
        });
        PrepStartTurn();
        await FinishUpAsync(isBeginning);
    }
    private async Task GameOverAsync()
    {
        _model.Score = 0;
        _model.RollNumber = 0;
        if (PlayerList.Any(xx => xx.Phase == 11) == false)
        {
            throw new CustomBasicException("Somebody had to complete the 10th phase in order to figure out who won");
        }
        SingleInfo = PlayerList.Where(xx => xx.Phase == 11).OrderByDescending(xx => xx.ScoreGame).Take(1).Single();
        await ShowWinAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "boardclicked":
                await BoardProcessAsync();
                return;
            case "calculate":
                await DoCalculateAsync();
                return;
            case "continueroll":
                await ContinueRollAsync();
                return;
            case "rolldice":
                BasicList<BasicList<RummyDiceInfo>> thisList = await js1.DeserializeObjectAsync<BasicList<BasicList<RummyDiceInfo>>>(content);
                await ShowRollingAsync(thisList);
                return;
            case "diceclicked":
                await SelectOneMainAsync(int.Parse(content));
                return;
            case "setchosen":
                await SetProcessAsync(int.Parse(content));
                return;
            case "diceset":
                SendSet thisSend = await js1.DeserializeObjectAsync<SendSet>(content);
                RummyDiceHandVM thisTemp = TempSets!.Single(xx => xx.Index == thisSend.WhichSet);
                await thisTemp.SelectUnselectDiceAsync(thisSend.Dice);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override Task ContinueTurnAsync()
    {
        RummyDistributionClass.CurrentPlayer = SingleInfo;
        return base.ContinueTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        SaveRoot!.RollNumber = 1;
        _model.Score = 0;
        TempSets!.ForEach(thisSet =>
        {
            thisSet.HandList.Clear();
        });
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        int nums = CalculateScore();
        _model.Score = nums;
        if (_test.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        MainBoard1!.EndTurn();
        UpdateInfo(nums, out bool Overs);
        if (Overs == true)
        {
            await GameOverAsync();
            return;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        await StartNewTurnAsync();
    }
    private void UpdateInfo(int score, out bool gameOver)
    {
        int previousScore;
        previousScore = SingleInfo!.ScoreGame;
        SingleInfo.ScoreRound = score;
        int newScore;
        newScore = previousScore + score;
        int phase;
        phase = SingleInfo.Phase;
        if (score == 0)
        {
            SingleInfo.HowManyRepeats++;
            SingleInfo.CurrentRepeats ++;
            if (SaveRoot!.SomeoneFinished == true)
            {
                SingleInfo.InGame = false;
            }
            gameOver = PlayerList.All(xx => xx.InGame == false);
            return;
        }
        if (phase == 5 && newScore > 250)
        {
            newScore += 40;
        }
        SingleInfo.Phase++;
        SingleInfo.CurrentRepeats = 0;
        if (phase == 10)
        {
            SaveRoot!.SomeoneFinished = true;
            SingleInfo.InGame = false;
            if (LeastRepeats() == true)
            {
                newScore += 40;
            }
        }
        SingleInfo.ScoreGame = newScore;
        gameOver = PlayerList.All(xx => xx.InGame == false);
    }
    public async Task DoCalculateAsync()
    {
        int nums = CalculateScore();
        _model.Score = nums;
        await ContinueTurnAsync();
    }
    private int CalculateScore()
    {
        int phase = SingleInfo!.Phase;
        PhaseList ThisPhase = PhaseInfo![phase - 1];
        int output;
        BasicList<RummyDiceInfo> thisCollection;
        BasicList<RummyDiceInfo> tempCollection;
        output = 0;
        for (int x = 1; x <= 2; x++)
        {
            thisCollection = [];
            tempCollection = TempSets![x - 1].HandList;
            if (tempCollection.Count > 0)
            {
                thisCollection.AddRange(tempCollection);
            }
            if (thisCollection.Count > 0)
            {
                foreach (var newSet in ThisPhase.PhaseSets)
                {
                    if (newSet.DidSucceed == false)
                    {
                        if (newSet.SetType == EnumWhatSets.Colors)
                        {
                            newSet.DidSucceed = Rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Colors);
                        }
                        else if (newSet.SetType == EnumWhatSets.Kinds)
                        {
                            newSet.DidSucceed = Rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Sets);
                        }
                        else if (newSet.SetType == EnumWhatSets.Runs)
                        {
                            newSet.DidSucceed = Rummys!.IsNewRummy(thisCollection, newSet.HowMany, EnumRummyType.Runs);
                        }
                        if (newSet.DidSucceed == true)
                        {
                            output += thisCollection.Sum(Items => Items.Value);
                            break;
                        }
                    }
                }
            }
        }
        if (ThisPhase.PhaseSets.Any(Items => Items.DidSucceed == false))
        {
            ResetSuccess();
            return 0;
        }
        ResetSuccess();
        return output;
    }
    private void ResetSuccess()
    {
        PhaseInfo!.ForEach(thisPhase =>
        {
            thisPhase.PhaseSets.ForEach(thisSet => thisSet.DidSucceed = false);
        });
    }
    private bool LeastRepeats()
    {
        int repeats = SingleInfo!.HowManyRepeats;
        return !PlayerList.Any(items => items.HowManyRepeats < repeats);
    }
    public async Task RollDiceAsync()
    {
        var thisCol = MainBoard1!.RollDice();
        if (BasicData.MultiPlayer == true)
        {
            if (thisCol.Count > 0)
            {
                await Network!.SendAllAsync("rolldice", thisCol);
            }
            else
            {
                await Network!.SendAllAsync("continueroll");
            }
        }
        await ShowRollingAsync(thisCol);
    }
    private async Task ShowRollingAsync(BasicList<BasicList<RummyDiceInfo>> thisList)
    {
        await MainBoard1!.ShowRollingAsync(thisList);
        await ContinueRollAsync();
    }
    private async Task ContinueRollAsync()
    {
        SaveRoot!.RollNumber++;
        await ContinueTurnAsync();
    }
    public async Task BoardProcessAsync()
    {
        BasicList<RummyDiceInfo> thisCol = [];
        TempSets!.ForEach(items =>
        {
            thisCol.AddRange(items.GetSelectedDiceAndRemove());
        });
        MainBoard1!.AddBack(thisCol);
        await ContinueTurnAsync();
    }
    public async Task SetProcessAsync(int whichOne)
    {
        BasicList<RummyDiceInfo> thisCol = [];
        thisCol.AddRange(MainBoard1!.GetSelectedList());
        TempSets!.ForConditionalItems(items => items.Index != whichOne, items =>
        {
            thisCol.AddRange(items.GetSelectedDiceAndRemove());
        });
        RummyDiceHandVM fins = TempSets.Single(items => items.Index == whichOne);
        fins.TransferTiles(thisCol);
        await ContinueTurnAsync();
    }
    public async Task SelectOneMainAsync(int whichOne)
    {
        MainBoard1!.SelectDice(whichOne);
        await ContinueTurnAsync();
    }
}