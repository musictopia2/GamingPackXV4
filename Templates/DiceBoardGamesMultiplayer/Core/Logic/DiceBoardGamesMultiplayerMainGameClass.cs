namespace DiceBoardGamesMultiplayer.Core.Logic;
[SingletonGame]
public class DiceBoardGamesMultiplayerMainGameClass
    : BoardDiceGameClass<DiceBoardGamesMultiplayerPlayerItem, DiceBoardGamesMultiplayerSaveInfo, EnumColorChoice, int>
    , IMiscDataNM, ISerializable
{
    private readonly DiceBoardGamesMultiplayerVMData _model;
    public DiceBoardGamesMultiplayerMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        DiceBoardGamesMultiplayerVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        DiceBoardGamesMultiplayerGameContainer container,
        StandardRollProcesses<SimpleDice, DiceBoardGamesMultiplayerPlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller, error, toast)
    {
        _model = model;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        BoardGameSaved(); //i think.
        SaveRoot.LoadMod(_model); //we usually need this.
                                  //anything else needed is here.
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true; //i think needs to be here.
    }
    protected override async Task ComputerTurnAsync()
    {
        //if there is nothing, then just won't do anything.
        await Task.CompletedTask;
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        SetUpDice();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SaveRoot.LoadMod(_model); //we usually need this.
        SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
        await FinishUpAsync(isBeginning);
    }

    Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.

            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();//if you did not choose colors, no need to prepstart because something else will do it.
            //code to run but only if you actually chose color.
        }

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override Task ContinueTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            //can do extra things upon continue turn.  many board games require other things.

        }
        return base.ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(int space)
    {
        //well see what we need for the move.
        await Task.CompletedTask;
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        //if anything else is needed, do here.
        if (PlayerList.DidChooseColors())
        {
            //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.

        }
        await StartNewTurnAsync();
    }

    public override async Task AfterChoosingColorsAsync()
    {
        //anything else that is needed after they finished choosing colors.

        await EndTurnAsync();
    }
    public override async Task AfterRollingAsync()
    {

        //anything needed after rolling is here.
        await ContinueTurnAsync();
    }
}