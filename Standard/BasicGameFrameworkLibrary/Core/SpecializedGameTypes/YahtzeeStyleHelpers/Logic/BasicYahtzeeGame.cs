﻿namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;

public class BasicYahtzeeGame<D> : DiceGameClass<D, YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>
where D : SimpleDice, new()
{
    private readonly YahtzeeGameContainer<D> _gameContainer;
    private readonly IScoreLogic _scoreLogic;
    private readonly ScoreContainer _scoreContainer;
    private readonly IYahtzeeEndRoundLogic _endRoundLogic;
    private readonly YahtzeeVMData<D> _model;
    public BasicYahtzeeGame(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        YahtzeeVMData<D> currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        YahtzeeGameContainer<D> gameContainer,
        IScoreLogic scoreLogic,
        ScoreContainer scoreContainer,
        IYahtzeeEndRoundLogic endRoundLogic,
        StandardRollProcesses<D, YahtzeePlayerItem<D>> roller,
        ISystemError error, IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _gameContainer = gameContainer;
        _scoreLogic = scoreLogic;
        _scoreContainer = scoreContainer;
        _endRoundLogic = endRoundLogic;
        _model = currentMod;
        _scoreContainer.StartTurn = () => SingleInfo!.MissNextTurn = false;
    }
    protected override bool ShowDiceUponAutoSave => false; //try this way.
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true); //could be.
        _model.Cup!.CanShowDice = false;
    }
    private async Task PrepTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        _scoreContainer.RowList = SingleInfo.RowList;
        if (_gameContainer.GetNewScoreAsync != null)
        {
            await _gameContainer.GetNewScoreAsync.Invoke(); //if nobody is handling it, then skip it.
        }
        _scoreLogic.StartTurn();
        ProtectedStartTurn();
    }
    private void LoadGame()
    {
        LoadMod();
        PlayerList.ForEach(player =>
        {
            if (player.RowList.Count == 0)
            {
                _scoreContainer.RowList = player.RowList.ToBasicList(); //try this way.  hopefully will allow cloning back later.
                _scoreLogic.LoadBoard();
                player.RowList = _scoreContainer.RowList.ToBasicList();
            }
        });
        SaveRoot.LoadMod(_model);
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadGame();
        AfterRestoreDice();
        SingleInfo = PlayerList.GetWhoPlayer();
        _scoreContainer.RowList = SingleInfo.RowList;
        if (SaveRoot!.RollNumber > 1)
        {
            _model.Cup!.CanShowDice = true;
            _scoreLogic.PopulatePossibleScores();
        }
        else
        {
            _model.Cup!.HideDice();
        }
        SaveRoot.LoadMod(_model);
        SingleInfo = PlayerList.GetWhoPlayer();
        if (_gameContainer.GetNewScoreAsync != null)
        {
            await _gameContainer.GetNewScoreAsync.Invoke(); //if nobody is handling it, then skip it.
        }
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        PlayerList!.ForEach(items =>
        {
            items.Points = 0;
            items.RowList.Clear();
        });
        LoadGame();
        SetUpDice();
        SaveRoot!.Round = 1;
        SaveRoot.Begins = WhoStarts;
        await PrepTurnAsync();
        await FinishUpAsync!(isBeginning);
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        _scoreLogic.PopulatePossibleScores();
        await ContinueTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        await PrepTurnAsync();
        if (_endRoundLogic.IsGameOver || Test!.ImmediatelyEndGame)
        {
            await EndTurnAsync();
            await _endRoundLogic.StartNewRoundAsync();
            return;
        }
        this.ShowTurn();
        await ContinueTurnAsync();
    }
}