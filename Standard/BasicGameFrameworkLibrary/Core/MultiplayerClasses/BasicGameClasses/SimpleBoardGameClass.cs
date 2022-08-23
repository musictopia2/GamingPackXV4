namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
public abstract class SimpleBoardGameClass<P, S, E, M> : BasicGameClass<P, S>, IMoveProcesses<M>,
    IBeginningColors<E, P, S>
    , IMoveNM, IAfterColorProcesses

    where E : IFastEnumColorSimple
     where P : class, IPlayerBoardGame<E>, new()

    where S : BasicSavedGameClass<P>, new()
{
    private readonly ISimpleBoardGamesData _currentMod;
    public SimpleBoardGameClass(IGamePackageResolver mainContainer,
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
            toast
            )
    {
        _currentMod = currentMod;
    }
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
        _currentMod.Instructions = "None";
        await base.ShowWinAsync();
        EraseColors(); //try here instead.
    }
    public override async Task ShowTieAsync()
    {
        _currentMod.Instructions = "None";
        await base.ShowTieAsync();
        EraseColors(); //try here as well
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
            return; //because you already chose colors.
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
        PlayerList.EraseColors(); //should be this simple.  just for convenience.  maybe something else will do it (not sure).
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
            item = await js.DeserializeObjectAsync<M>(data);
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