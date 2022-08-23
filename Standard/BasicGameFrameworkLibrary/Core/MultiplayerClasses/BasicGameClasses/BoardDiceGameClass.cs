﻿namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;

public abstract class BoardDiceGameClass<P, S, E, M> : SimpleBoardGameClass<P, S, E, M>,
    IStandardRoller<SimpleDice, P>,
    IBeginningColors<E, P, S>
    where E : IFastEnumColorSimple
     where P : class, IPlayerBoardGame<E>, new()
    where S : BasicSavedBoardDiceGameClass<P>, new()
{
    private readonly IDiceBoardGamesData _model;
    public BoardDiceGameClass(
        IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        IDiceBoardGamesData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        BasicGameContainer<P, S> gameContainer,
        StandardRollProcesses<SimpleDice, P> roller,
        ISystemError error,
        IToast toast
        ) : base(
            mainContainer,
            aggregator,
            basicData,
            test,
            currentMod,
            state,
            delay,
            command,
            gameContainer,
            error,
            toast)
    {
        _model = currentMod;
        Roller = roller;
        Roller.AfterRollingAsync = AfterRollingAsync;
        Roller.AfterSelectUnselectDiceAsync = AfterSelectUnselectDiceAsync;
        Roller.CurrentPlayer = () => SingleInfo!;
    }
    public DiceCup<SimpleDice>? Cup => _model.Cup;
    protected StandardRollProcesses<SimpleDice, P> Roller { get; }
    public Task AfterSelectUnselectDiceAsync()
    {
        return Task.CompletedTask;
    }
    public abstract Task AfterRollingAsync();
    protected void SetUpDice()
    {
        LoadCup(false);
        SaveRoot.DiceList.MainContainer = MainContainer; //maybe has to be here.
    }
    protected virtual bool ShowDiceUponAutoSave => true;
    protected void AfterRestoreDice()
    {
        LoadCup(true);
        SaveRoot!.DiceList.MainContainer = MainContainer; //maybe has to be here.
        if (ShowDiceUponAutoSave == true)
        {
            _model.Cup!.CanShowDice = true;
            _model.Cup.ShowDiceListAlways = true;
            _model.Cup.Visible = true;
        }
        else
        {
            _model.Cup!.CanShowDice = false;
        }
    }
    protected virtual void LoadCup(bool autoResume)
    {
        _model.LoadCup(SaveRoot, autoResume);
    }
}