﻿namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
public abstract class SimpleBoardGameClass<P, S, E, M>(IGamePackageResolver mainContainer,
    IEventAggregator aggregator,
    BasicData basicData,
    TestOptions test,
    ISimpleBoardGamesData currentMod,
    IMultiplayerSaveState state,
    IAsyncDelayer delay,
    CommandContainer command,
    BasicGameContainer<P, S> gameContainer,
    ISystemError error,
    IToast toast
        ) : BasicGameClass<P, S>(
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
        toast
            ), IMoveProcesses<M>,
    IBeginningColors<E, P, S>
    , IMoveNM, IAfterColorProcesses

    where E : IFastEnumColorSimple
     where P : class, IPlayerBoardGame<E>, new()

    where S : BasicSavedGameClass<P>, new()
{
    public abstract Task MakeMoveAsync(M space);
    protected override async Task ComputerTurnAsync()
    {
        if (PlayerList.DidChooseColors() == false)
        {
            if (MiscDelegates.ComputerChooseColorsAsync == null)
            {
                throw new CustomBasicException("The computer choosing color was never handled.  Rethink");
            }
            await MiscDelegates.ComputerChooseColorsAsync.Invoke();
        }
    }
    public override async Task ShowWinAsync()
    {
        currentMod.Instructions = "None";
        await base.ShowWinAsync();
        EraseColors();
    }
    public override async Task ShowTieAsync()
    {
        currentMod.Instructions = "None";
        await base.ShowTieAsync();
        EraseColors();
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning => PlayerList.DidChooseColors();
    protected bool CanPrepTurnOnSaved { get; set; } = true;
    protected void BoardGameSaved()
    {
        if (CanPrepTurnOnSaved)
        {
            PrepStartTurn();
        }
    }
    protected override async Task ShowHumanCanPlayAsync()
    {
        await base.ShowHumanCanPlayAsync();
        if (PlayerList.DidChooseColors() == false && MiscDelegates.ManuelSetColors != null)
        {
            MiscDelegates.ManuelSetColors.Invoke();
        }
    }
    protected override async Task LoadPossibleOtherScreensAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            return;
        }
        if (SingleInfo == null)
        {
            throw new CustomBasicException("Single info cannot be null when trying to load other possible screens.  Rethink");
        }
        if (MiscDelegates.ContinueColorsAsync == null)
        {
            return;
        }
        await MiscDelegates.ContinueColorsAsync.Invoke();
    }
    public void EraseColors()
    {
        PlayerList.EraseColors();
    }
    async Task IMoveNM.MoveReceivedAsync(string data)
    {
        M item;
        if (typeof(M) == typeof(int))
        {
            object temp = int.Parse(data);
            item = (M)temp;
        }
        else
        {
            item = await js1.DeserializeObjectAsync<M>(data);
        }

        await MakeMoveAsync(item);
    }
    /// <summary>
    /// by this time, something else already loaded the proper screens.
    /// this has to decide if anything else is needed like on game of life, loading yet another screen.
    /// </summary>
    /// <returns></returns>
    public abstract Task AfterChoosingColorsAsync();
}