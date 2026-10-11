namespace BasicMultiplayerGames.Core.Logic;
[SingletonGame]
public class BasicMultiplayerGamesMainGameClass(IGamePackageResolver resolver,
    IEventAggregator aggregator,
    BasicData basic,
    TestOptions test,
    BasicMultiplayerGamesVMData model,
    IMultiplayerSaveState state,
    IAsyncDelayer delay,
    CommandContainer command,
    IRandomGenerator rs,
    BasicMultiplayerGamesGameContainer gameContainer,
    ISystemError error,
    IToast toast
        ) : BasicGameClass<BasicMultiplayerGamesPlayerItem, BasicMultiplayerGamesSaveInfo>(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    , ICommonMultiplayer<BasicMultiplayerGamesPlayerItem, BasicMultiplayerGamesSaveInfo>
    , IMiscDataNM
{
    private readonly BasicMultiplayerGamesVMData? _model = model;
    private readonly IRandomGenerator _rs = rs; //if we don't need, take out.

    public override Task FinishGetSavedAsync()
    {
        LoadControls();
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
    //change to how you end the turn.  otherwise, nothing happens.
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        await StartNewTurnAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        //if there is nothing, then just won't do anything.
        await Task.CompletedTask;
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
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
}