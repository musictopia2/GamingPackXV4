namespace Bingo.Core.Logic;
[SingletonGame]
public class BingoMainGameClass : BasicGameClass<BingoPlayerItem, BingoSaveInfo>
    , ICommonMultiplayer<BingoPlayerItem, BingoSaveInfo>
    , IMiscDataNM
{
    internal BingoItem? CurrentInfo { get; set; }
    private int _currentNum;
    public BingoMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        BingoVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        IRandomGenerator rs,
        BingoGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _model = model;
        _rs = rs;
    }
    private readonly BingoVMData? _model;
    private readonly IRandomGenerator _rs; //if we don't need, take out.
    internal Action<bool>? SetTimerEnabled { get; set; }
    public override async Task FinishGetSavedAsync()
    {
        if (Network == null)
        {
            throw new CustomBasicException("The network was never created.  Rethink");
        }
        LoadControls();
        PopulateOwn();
        _currentNum = 0;
        await CallNextNumberAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }

        IsLoaded = true; //i think needs to be here.
    }
    internal async Task FinishAsync()
    {
        await ComputerTurnAsync();
    }
    public override Task ContinueTurnAsync()
    {
        return Task.CompletedTask; //because no turns this time.
    }
    protected override async Task ComputerTurnAsync()
    {
        await Task.Delay(10);
        var tempList = PlayerList.Where(items => items.PlayerCategory == EnumPlayerCategory.Computer).ToBasicList();
        foreach (var thisPlayer in tempList)
        {
            BingoItem bingo = thisPlayer.BingoList.FirstOrDefault(Items => Items.WhatValue == CurrentInfo!.WhatValue)!;
            if (bingo != null)
            {
                bingo.DidGet = true;
                if (thisPlayer.BingoList.HasBingo == true)
                {
                    if (BasicData!.MultiPlayer == true)
                    {
                        await Network!.SendAllAsync("bingo", thisPlayer.Id);
                    }
                    await GameOverAsync(thisPlayer.Id);
                    break;
                }
            }
        }
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendAllAsync("callnextnumber");
        }
        await CallNextNumberAsync();
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        BasicList<int> CardList = _rs!.GenerateRandomList(75);
        SaveRoot!.CallList.Clear();
        SaveRoot.BingoBoard.ClearBoard(_model!); //i think.
        CardList.ForEach(x =>
        {
            CurrentInfo = new();
            CurrentInfo.WhatValue = x;
            CurrentInfo.Vector = new(0, MatchNumber(x));
            CurrentInfo.Letter = WhatLetter(CurrentInfo.Vector.Column);
            SaveRoot.CallList.Add(SaveRoot.CallList.Count + 1, CurrentInfo);
        });
        CreateBingoCards();
        await FinishUpAsync(isBeginning);
        await CallNextNumberAsync(); //maybe i can have here this time.
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "bingo":
                await GameOverAsync(int.Parse(content));
                break;
            case "callnextnumber":
                try
                {
                    await CallNextNumberAsync();
                }
                catch (Exception)
                {
                    //this time, ignore errors.  if that works i can continue.
                }
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override Task StartNewTurnAsync()
    {
        throw new CustomBasicException("I don't think one starts new turn.  If I am wrong. rethink");
    }
    private void CreateBingoCards()
    {
        _currentNum = 0;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.BingoList.ClearBoard();
            NewBingoCard(thisPlayer);
        });
        PopulateOwn();
    }
    private void PopulateOwn()
    {
        SingleInfo = PlayerList!.GetSelf();
        var tempList = SingleInfo.BingoList.Where(ThisBingo => ThisBingo.WhatValue > 0).ToBasicList();
        tempList.ForEach(thisBingo =>
        {
            var boardItem = SaveRoot!.BingoBoard[thisBingo.Vector.Row + 1, thisBingo.Vector.Column];
            boardItem.Text = thisBingo.WhatValue.ToString();
        });
        var finList = SaveRoot!.BingoBoard.Where(Items => Items.Vector.Row == 1).ToBasicList();
        finList.ForEach(thisBingo =>
        {
            thisBingo.Text = WhatLetter(thisBingo.Vector.Column);
        });
        SaveRoot.BingoBoard[4, 3].Text = "Free"; //not sure why its not working properly.  try to do here too (?)
    }
    private void NewBingoCard(BingoPlayerItem thisPlayer)
    {
        BasicList<int> thisList;
        int x;
        int y;
        BingoItem thisBingo;
        int starts = 1;
        for (x = 1; x <= 5; x++)
        {
            if (x != 3)
                y = 5;
            else
                y = 4;
            thisList = _rs!.GenerateRandomList(starts + 14, y, starts);
            if (x == 3)
            {
                for (y = 1; y <= 2; y++)
                {
                    thisBingo = thisPlayer.BingoList[y, x];
                    thisBingo.DidGet = false;
                    thisBingo.Letter = WhatLetter(x);
                    thisBingo.WhatValue = thisList[y - 1];
                }
                thisBingo = thisPlayer.BingoList[3, 3];
                thisBingo.Letter = "N";
                thisBingo.WhatValue = 0;
                if (thisPlayer.PlayerCategory == EnumPlayerCategory.Computer)
                {
                    thisBingo.DidGet = true;
                }
                for (y = 3; y <= 4; y++)
                {
                    thisBingo = thisPlayer.BingoList[y + 1, x];
                    thisBingo.DidGet = false;
                    thisBingo.Letter = WhatLetter(x);
                    thisBingo.WhatValue = thisList[y - 1];
                }
            }
            else
            {
                for (y = 1; y <= 5; y++)
                {
                    thisBingo = thisPlayer.BingoList[y, x];
                    thisBingo.DidGet = false;
                    thisBingo.Letter = WhatLetter(x);
                    thisBingo.WhatValue = thisList[y - 1];
                }
            }
            starts += 15;
        }
    }
    public static string WhatLetter(int column) //iffy.
    {
        switch (column)
        {
            case 1:
                {
                    return "B";
                }

            case 2:
                {
                    return "I";
                }

            case 3:
                {
                    return "N";
                }

            case 4:
                {
                    return "G";
                }

            default:
                {
                    return "O";
                }
        }
    }
    public static int MatchNumber(int index)
    {
        switch (index)
        {
            case object _ when index < 16:
                {
                    return 1;
                }

            case object _ when index < 31:
                {
                    return 2;
                }

            case object _ when index < 46:
                {
                    return 3;
                }

            case object _ when index < 61:
                {
                    return 4;
                }

            default:
                {
                    return 5;
                }
        }
    }
    public async Task GameOverAsync(int player)
    {
        SingleInfo = PlayerList![player];
        if (SetTimerEnabled == null)
        {
            throw new CustomBasicException("The timer processes was never set.  Rethink");
        }
        SetTimerEnabled.Invoke(false);
        await ShowWinAsync();
    }
    internal async Task CallNextNumberAsync()
    {
        if (SetTimerEnabled == null)
        {
            return;
        }
        _currentNum++;
        if (_currentNum > 75)
        {
            throw new CustomBasicException("Cannot go higher than 75");
        }
        CurrentInfo = SaveRoot!.CallList[_currentNum];
        _model!.NumberCalled = $"{CurrentInfo.Letter}{CurrentInfo.WhatValue}";
        SingleInfo = PlayerList!.GetSelf();
        if (Test!.DoubleCheck == false)
        {
            SetTimerEnabled.Invoke(true);
            await ShowHumanCanPlayAsync();
        }
        else
        {
            //await RunTestAsync();
        }
    }
}