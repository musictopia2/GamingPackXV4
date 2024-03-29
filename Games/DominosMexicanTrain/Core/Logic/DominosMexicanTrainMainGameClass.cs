
namespace DominosMexicanTrain.Core.Logic;
[SingletonGame]
public class DominosMexicanTrainMainGameClass : DominosGameClass<MexicanDomino, DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>
    , ICommonMultiplayer<DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>
    , IMiscDataNM, ISerializable, IStartNewGame
{
    public DominosMexicanTrainMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        DominosMexicanTrainVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        GlobalClass global,
        DominosMexicanTrainGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _model = model;
        _command = command;
        _global = global;
        DominosToPassOut = 12;
    }
    private readonly DominosMexicanTrainVMData _model;
    private readonly CommandContainer _command;
    private readonly GlobalClass _global;
    internal async Task GameOverAsync()
    {
        SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
        await ShowWinAsync();
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        ProtectedLoadBone();
        PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.LongestTrainList.Count > 0)
            {
                DeckRegularDict<MexicanDomino> tempList = new();
                _model.BoneYard!.LoadPlayerPieces(thisPlayer.LongestTrainList, ref tempList, false);
                thisPlayer.MainHandList.AddRange(tempList);
                thisPlayer.LongestTrainList.Clear();
            }
            thisPlayer.MainHandList.ForEach(thisDomino =>
            {
                thisDomino.Drew = false;
                thisDomino.IsSelected = false;
            });
        });
        SingleInfo = PlayerList.GetSelf();
        _model.TrainStation1!.Self = SingleInfo.Id;
        _model.PrivateTrain1!.ClearHand();
        _model.UpdateCount(SingleInfo);
        SingleInfo.MainHandList.Sort();
        SingleInfo.DoInit(Aggregator);
        SaveRoot!.LoadMod(_model);
        await _model.TrainStation1.LoadSavedDataAsync(SaveRoot);
        SingleInfo = PlayerList.GetWhoPlayer();
        AfterPassedDominos();
    }
    private void FirstLoad()
    {
        int highest;
        if (PlayerList.Count < 5)
        {
            highest = 12;
        }
        else
        {
            highest = 15;
        }
        _model.TrainStation1!.Self = PlayerList!.GetSelf().Id;
        _model.TrainStation1.LoadPlayers(highest, this);
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        LoadUpDominos();
        FirstLoad();
        _global.Animates = new(Aggregator);
        _global.Animates.LongestTravelTime = 75;
        _model.PrivateTrain1.Text = "Your Train";
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

        SaveRoot!.LoadMod(_model);
        _model.PrivateTrain1!.HandList.Clear(); //i think
        SaveRoot.FirstPlayerPlayed = 0;
        SaveRoot.ImmediatelyStartTurn = true;
        ClearBoneYard();
        if (isBeginning == false)
        {
            _model.TrainStation1!.NewRound();
        }
        else
        {
            _model.TrainStation1!.StartRound();
        }
        PassDominos();
        if (isBeginning)
        {
            SingleInfo = PlayerList!.GetSelf();
            SingleInfo.MainHandList.Sort();
            SingleInfo.DoInit(Aggregator);
        }
        PlayerList!.ForEach(thisPlayer => thisPlayer.LongestTrainList.Clear());
        SingleInfo = PlayerList.GetSelf();
        _model.UpdateCount(SingleInfo); //try this too.
        SingleInfo = PlayerList.GetWhoPlayer();
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            case "dominoplayed":
                SendPlay output = await js1.DeserializeObjectAsync<SendPlay>(content);
                MexicanDomino thisDomino = SingleInfo!.MainHandList.GetSpecificItem(output.Deck);
                SingleInfo.MainHandList.RemoveObjectByDeck(output.Deck);
                _command.UpdateAll(); //i think.
                await _model.TrainStation1!.AnimateShowSelectedDominoAsync(output.Section, thisDomino, this);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public async Task EndTurnAsync(bool didPlay)
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.PlayerHand1!.EndTurn();
            _model.PrivateTrain1!.EndTurn();
        }
        if (didPlay == false)
        {
            _model.TrainStation1!.PutTrain(WhoTurn, PlayerList);
        }
        if (await CanEndRoundAsync(didPlay))
        {
            await EndRoundAsync();
            return;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        SaveRoot!.ImmediatelyStartTurn = false; //otherwise, will give overflow error.
        ProtectedStartTurn();
        SingleInfo = PlayerList!.GetWhoPlayer();
        SaveRoot.CurrentPlayerDouble = false;
        Aggregator.RepaintBoard();
        await ContinueTurnAsync();
    }
    public override Task PopulateSaveRootAsync()
    {
        ProtectedSaveBone();
        _model.TrainStation1.SavedData();
        return Task.CompletedTask;
    }
    private async Task<bool> CanEndRoundAsync(bool didPlay)
    {
        if (Test!.EndRoundEarly && (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self || Test.ComputerEndsTurn == false))
        {
            return true; //double check should mean something else.
        }
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.ObjectCount == 0)
        {
            return true;
        }
        if (didPlay)
        {
            SaveRoot!.FirstPlayerPlayed = 0;
            return false;
        }
        if (SaveRoot!.FirstPlayerPlayed == 0)
        {
            if (_model.TrainStation1!.CanFillPrevious(PlayerList, WhoTurn))
            {
                SaveRoot.FirstPlayerPlayed = WhoTurn;
            }
            return false;
        }
        if (_model.TrainStation1!.CanEndEarly() == false)
        {
            SaveRoot.FirstPlayerPlayed = 0;
            return false;
        }
        int newTurn = await PlayerList.CalculateWhoTurnAsync();
        return newTurn == SaveRoot.FirstPlayerPlayed;
    }
    private static int CalculatePoints(DominosMexicanTrainPlayerItem thisPlayer)
    {
        DeckRegularDict<MexicanDomino> tempList = thisPlayer.MainHandList.ToRegularDeckDict();
        tempList.AddRange(thisPlayer.LongestTrainList);
        return tempList.Sum(items => items.Points);
    }
    private async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.PreviousLeft = thisPlayer.ObjectCount;
            thisPlayer.PreviousScore = CalculatePoints(thisPlayer);
            thisPlayer.TotalScore += thisPlayer.PreviousScore;
        });
        _command.UpdateAll(); //has to update all so a person can see the scores.
        await _model.TrainStation1!.EndRoundAsync(this);
    }
    public override async Task EndTurnAsync()
    {
        await EndTurnAsync(false);
    }
    public override Task PlayDominoAsync(int Deck)
    {
        throw new CustomBasicException("I don't think we can run it this time.");
    }
    public bool ForceSatisfy()
    {
        if (_model.TrainStation1!.NeedDouble(out int numberNeeded) == false)
        {
            return false;
        }
        if (SaveRoot!.CurrentPlayerDouble == true)
        {
            return false;
        }
        if (numberNeeded < 0)
        {
            throw new CustomBasicException("The number neeed has to at least be 0");
        }
        foreach (var thisPlayer in PlayerList!)
        {
            if (thisPlayer.ObjectCount < 5 && WhoTurn != thisPlayer.Id)
            {
                return false;
            }
        }
        DeckRegularDict<MexicanDomino> tempList = SingleInfo!.MainHandList.ToRegularDeckDict();
        tempList.AddRange(SingleInfo.LongestTrainList);
        return tempList.Any(items => items.FirstNum == numberNeeded || items.SecondNum == numberNeeded);
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.PreviousScore = 0;
            thisPlayer.PreviousLeft = 0;
        });
        SaveRoot!.UpTo = 12;
        return Task.CompletedTask;
    }
}