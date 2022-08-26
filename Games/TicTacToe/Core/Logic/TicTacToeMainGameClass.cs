namespace TicTacToe.Core.Logic;
[SingletonGame]
public class TicTacToeMainGameClass : BasicGameClass<TicTacToePlayerItem, TicTacToeSaveInfo>
    , ICommonMultiplayer<TicTacToePlayerItem, TicTacToeSaveInfo>
    , IMoveNM, ISerializable
{
    public TicTacToeMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        TicTacToeVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        TicTacToeGameContainer gameContainer,
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
        PlayerList.First().Piece = EnumSpaceType.X;
        PlayerList.Last().Piece = EnumSpaceType.O;
        SaveRoot!.GameBoard.Clear(); //i think here too.
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn(); //anything else is below.
        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override async Task EndTurnAsync()
    {
        _command.ManuelFinish = true;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public async Task MakeMoveAsync(SpaceInfoCP space)
    {
        SingleInfo = SaveRoot!.PlayerList.GetWhoPlayer();
        space.Status = SingleInfo.Piece;
        WinInfo thisWin = SaveRoot.GameBoard.GetWin();
        if (BasicData!.MultiPlayer == true && SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            await Network!.SendMoveAsync(space);
        }
        if (thisWin.WinList.Count > 0)
        {
            await ShowWinAsync();
            return;
        }
        if (thisWin.IsDraw == true)
        {
            await ShowTieAsync();
            return;
        }
        await EndTurnAsync();
    }
    async Task IMoveNM.MoveReceivedAsync(string data)
    {
        SpaceInfoCP tempMove = await js.DeserializeObjectAsync<SpaceInfoCP>(data);
        SpaceInfoCP realMove = SaveRoot!.GameBoard[tempMove.Vector]; //i think
        await MakeMoveAsync(realMove);
    }
}