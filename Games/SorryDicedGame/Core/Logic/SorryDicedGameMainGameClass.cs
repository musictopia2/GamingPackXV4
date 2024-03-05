
namespace SorryDicedGame.Core.Logic;
[SingletonGame]
public class SorryDicedGameMainGameClass
    : SimpleBoardGameClass<SorryDicedGamePlayerItem, SorryDicedGameSaveInfo, EnumColorChoice, int>
    , IBeginningColors<EnumColorChoice, SorryDicedGamePlayerItem, SorryDicedGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    public SorryDicedGameMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        SorryDicedGameVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        SorryDicedGameGameContainer gameContainer,
        ISystemError error,
        IToast toast,
        SorryCompleteDiceSet completeDice
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _model = model;
        _completeDice = completeDice;
    }
    private readonly SorryDicedGameVMData? _model;
    private readonly SorryCompleteDiceSet _completeDice;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        SorryDicedGameGameContainer.CanStart = PlayerList.DidChooseColors();
        BoardGameSaved(); //i think.
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
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SorryDicedGameGameContainer.CanStart = false;
        SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case "dicelist":
                var dice = await _completeDice.GetDiceList(content);
                await ShowRollingAsync(dice);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
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

        if (MiscDelegates.FillRestColors == null)
        {
            throw new CustomBasicException("Nobody is handling filling the rest of the colors.  Rethink");
        }
        MiscDelegates.FillRestColors.Invoke();
        SorryDicedGameGameContainer.CanStart = true;
        await EndTurnAsync();
    }

    public async Task RollAsync()
    {
        var firsts = _completeDice.RollDice();
        if (BasicData.MultiPlayer)
        {
            await _completeDice.SendMessageAsync("dicelist", firsts);
        }
        await ShowRollingAsync(firsts);
    }
    private async Task ShowRollingAsync(BasicList<BasicList<SorryDiceModel>> thisList)
    {
        await _completeDice.ShowRollingAsync(thisList);
        await ContinueTurnAsync();
    }

}