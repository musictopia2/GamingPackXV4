namespace BasicMultiplayerDiceGames.Core.Logic;
[SingletonGame]
public class BasicMultiplayerDiceGamesMainGameClass
    : DiceGameClass<SimpleDice, BasicMultiplayerDiceGamesPlayerItem, BasicMultiplayerDiceGamesSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly BasicMultiplayerDiceGamesVMData? _model;
    public BasicMultiplayerDiceGamesMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        BasicMultiplayerDiceGamesVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        BasicMultiplayerDiceGamesGameContainer gameContainer,
        StandardRollProcesses<SimpleDice, BasicMultiplayerDiceGamesPlayerItem> roller,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, currentMod, state, delay, command, gameContainer, roller, error, toast)
    {
        _model = currentMod;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        AfterRestoreDice();
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        LoadMod();
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
        PrepStartTurn(); //anything else is below.

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    protected override async Task ProtectedAfterRollingAsync()
    {
        //anything else that needs to happen after rolling happens here.
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        //anything else that needs to happen will be here.
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
}