namespace BowlingDiceGame.Core.Logic;
[SingletonGame]
public class BowlingDiceGameMainGameClass(IGamePackageResolver resolver,
    IEventAggregator aggregator,
    BasicData basic,
    TestOptions test,
    BowlingDiceGameVMData model,
    IMultiplayerSaveState state,
    IAsyncDelayer delay,
    CommandContainer command,
    BowlingDiceGameGameContainer gameContainer,
    ISystemError error,
    IToast toast
        ) : BasicGameClass<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    , ICommonMultiplayer<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>
    , IRolledNM,
    IMiscDataNM,
    ISerializable
{
    private readonly TestOptions _test = test;
    public BowlingScoresCP? ScoreSheets;
    public BowlingDiceSet? DiceBoard;
    public override async Task PopulateSaveRootAsync()
    {
        SaveRoot!.DiceData = await DiceBoard!.SaveGameAsync();
        RollContext.CurrentDistribution = SaveRoot.RollDistribution; //if null, okay.
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        await DiceBoard!.LoadGameAsync(SaveRoot!.DiceData);
    }
    public async Task RollReceivedAsync(string data)
    {
        if (ScoreSheets!.NeedToClear() == true)
        {
            DiceBoard!.ClearDice();
            await Delay!.DelaySeconds(0.1);
        }
        var thisList = await DiceBoard!.GetDiceList(data);
        await RollDiceAsync(thisList);
    }
    private async Task GameOverAsync()
    {
        SingleInfo = PlayerWon();
        await ShowWinAsync();
    }
    private BowlingDiceGamePlayerItem PlayerWon()
    {
        return PlayerList!.OrderByDescending(items => items.TotalScore).Take(1).Single();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        ScoreSheets = MainContainer.Resolve<BowlingScoresCP>();
        DiceBoard = MainContainer.Resolve<BowlingDiceSet>();
        DiceBoard.FirstLoad();
        IsLoaded = true;
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        SaveRoot!.WhatFrame = 1;
        if (isBeginning == true)
        {
            LoadPlayerFrames();
        }
        LoadControls();
        ScoreSheets!.ClearBoard();
        DiceBoard!.ClearDice();
        SaveRoot.WhichPart = 1;
        var list = EnumLuckProfile.CompleteList;
        list.ShuffleList();
        foreach (var item in PlayerList)
        {
            //item.LuckProfile = EnumLuckProfile.HotestStreak; //has to be hard coded until i fix bugs.
            item.LuckProfile = list.First();
            list.RemoveFirstItem();
        }
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        await FinishUpAsync(isBeginning);
    }
    private void LoadPlayerFrames()
    {
        PlayerList!.ForEach(items =>
        {
            10.Times(x =>
            {
                FrameInfoCP thisFrame = new();
                3.Times(y =>
                {
                    SectionInfoCP thisSection = new();
                    thisSection.Score = "0";
                    thisFrame.SectionList.Add(y, thisSection);
                });
                items.FrameList.Add(x, thisFrame);
            });
        });
    }
    public override async Task EndTurnAsync()
    {
        ScoreSheets!.UpdateScore();
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        if (WhoTurn == WhoStarts)
        {
            SaveRoot!.WhatFrame++;
            if (SaveRoot.WhatFrame > 10)
            {
                await GameOverAsync();
                return;
            }
        }
        await StartNewTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        DiceBoard!.ClearDice();
        SaveRoot.RollDistribution = null;
        SaveRoot!.WhichPart = 1;
        SaveRoot.IsExtended = false;
        await ContinueTurnAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (BasicData!.MultiPlayer == true && BasicData.Client == true)
        {
            throw new CustomBasicException("Clients can't go for the comptuer for multiplayer games");
        }
        if (_test!.NoAnimations == false)
        {
            await Delay!.DelayMilli(300);
        }
        if (SaveRoot!.WhichPart < 3)
        {
            await RollDiceAsync();
            return;
        }
        if (SaveRoot.IsExtended == true)
        {
            SaveRoot.IsExtended = false;
            await RollDiceAsync();
            return;
        }
        if (_test.NoAnimations == false)
        {
            await Delay!.DelayMilli(300);
        }
        if (BasicData.MultiPlayer == true)
        {
            await Network!.SendEndTurnAsync();
        }
        await EndTurnAsync();
    }
    public async Task RollDiceAsync(BasicList<BasicList<bool>> thisList)
    {
        await DiceBoard!.ShowRollingAsync(thisList);
        int previouss;
        int hits;
        hits = DiceBoard.HowManyHit();
        previouss = ScoreSheets!.PreviousHit();
        int newOnes;
        newOnes = hits - previouss;
        ScoreSheets.UpdateForSection(newOnes);
        SaveRoot!.WhichPart += 1;
        if (SaveRoot.WhichPart == 3)
        {
            if (ScoreSheets.CanExtend() == true)
            {
                SaveRoot.IsExtended = true; // extended is for ui only.
            }
            else
            {
                SaveRoot.IsExtended = false;
            }
        }
        else
        {
            SaveRoot.IsExtended = false;
        }
        await ContinueTurnAsync();
    }


    private async Task ProcessDistributionsAsync()
    {
        //RollContext.CurrentDistribution = null;
        int scoreForFrame = ScoreGenerator.GetFrameScoreBasedOnProfile(SingleInfo!.LuckProfile);
        if (scoreForFrame < 0 || scoreForFrame > 30)
        {
            throw new CustomBasicException("Score for frame must be between 0 and 30.  Rethink");
        }
        RollContext.CurrentDistribution = BowlingRollFactory.GetRollsForTotal(scoreForFrame);
        SaveRoot.RollDistribution = RollContext.CurrentDistribution; //so if the host takes their turn, remembers the state.
        if (SingleInfo!.CanSendMessage(BasicData!))
        {
            await Network!.SendAllAsync(nameof(RollDistribution), RollContext.CurrentDistribution);
        }
    }

    public async Task RollDiceAsync()
    {
        if (SaveRoot.WhichPart == 1)
        {
            //this means to figure out the distribution.
            await ProcessDistributionsAsync();

        }
        RollContext.CurrentRollNumber = SaveRoot.WhichPart;
        if (ScoreSheets!.NeedToClear() == true)
        {
            DiceBoard!.ClearDice();
            await Delay!.DelaySeconds(0.1);
        }
        var thisList = DiceBoard!.RollDice();
        if (SingleInfo!.CanSendMessage(BasicData!) == true)
        {
            await DiceBoard.SendMessageAsync(thisList);
        }
        await RollDiceAsync(thisList);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        if (status == nameof(RollDistribution))
        {
            SaveRoot.RollDistribution = js1.DeserializeObject<RollDistribution>(content) ?? throw new CustomBasicException("Received null distribution from network");

            await ContinueTurnAsync(); //i think needs to continue turn so it can wait for other messages.
            return;
        }
        throw new CustomBasicException("No other status is supported for now.  Rethink");
    }
}