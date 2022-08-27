namespace ConnectFour.Core.Logic;
[SingletonGame]
public class ConnectFourMainGameClass
    : SimpleBoardGameClass<ConnectFourPlayerItem, ConnectFourSaveInfo, EnumColorChoice, int>, ISerializable,
    IBeginningColors<EnumColorChoice, ConnectFourPlayerItem, ConnectFourSaveInfo>
{
    public ConnectFourMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        ConnectFourVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        ConnectFourGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _command = command;
    }

    private readonly CommandContainer _command;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        BoardGameSaved();
        RepaintBoard();
        return Task.CompletedTask;
    }
    private void RepaintBoard()
    {
        Aggregator.RepaintMessage(EnumRepaintCategories.Main);
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
        SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
        }

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override async Task MakeMoveAsync(int column)
    {
        if (BasicData!.MultiPlayer == true && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            await Network!.SendMoveAsync(column);
        }
        Vector topSpace = SaveRoot!.GameBoard[1, column].Vector;
        SpaceInfoCP tempSpace = new();
        tempSpace.Color = SingleInfo!.Color.ToString().ToColor();
        tempSpace.Player = WhoTurn;
        tempSpace.Vector = new Vector(1, column);
        Vector BottomSpace = SaveRoot.GameBoard.GetBottomSpace(column);
        if (BottomSpace.Row > 1)
        {
            await Aggregator.AnimateMovePiecesAsync(topSpace, BottomSpace, tempSpace, true);
        }
        SaveRoot.GameBoard[BottomSpace].Color = tempSpace.Color;
        SaveRoot.GameBoard[BottomSpace].Player = WhoTurn;
        RepaintBoard();
        WinInfo thisWin = SaveRoot.GameBoard.GetWin();
        if (thisWin.IsDraw == true)
        {
            await ShowTieAsync();
            return;
        }
        if (Test!.ImmediatelyEndGame)
        {
            await ShowTieAsync();
            return; //even if we need to immediately end game for testing, will show tie.
        }
        if (thisWin.WinList.Count > 0)
        {
            RepaintBoard();
            await ShowWinAsync();
            return;
        }
        await EndTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        _command!.ManuelFinish = true;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        SaveRoot!.GameBoard.Clear();
        RepaintBoard(); //not sure if we need to update all (?)
        await EndTurnAsync();
    }
}