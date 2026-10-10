namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
public abstract class BasicGameWithColorsClass<P, S, E>(IGamePackageResolver mainContainer,
    IEventAggregator aggregator,
    BasicData basicData,
    TestOptions test,
    IInstructionData currentMod,
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
            ),
    IBeginningColors<E, P, S>
    , IAfterColorProcesses

    where E : IFastEnumColorSimple
     where P : class, IPlayerWithColor<E>, new()

    where S : BasicSavedGameClass<P>, new()
{
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
    protected IInstructionData CurrentMod => currentMod; //may need it (not sure).  best to risk having than not having
    public override async Task ShowWinAsync()
    {
        CurrentMod.Instructions = "None";
        await base.ShowWinAsync();
        EraseColors();
    }
    public override async Task ShowTieAsync()
    {
        CurrentMod.Instructions = "None";
        await base.ShowTieAsync();
        EraseColors();
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning => PlayerList.DidChooseColors();
    protected bool CanPrepTurnOnSaved { get; set; } = true;
    protected void GameSaved()
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

    /// <summary>
    /// by this time, something else already loaded the proper screens.
    /// this has to decide if anything else is needed like on game of life, loading yet another screen.
    /// </summary>
    /// <returns></returns>
    public abstract Task AfterChoosingColorsAsync();
}