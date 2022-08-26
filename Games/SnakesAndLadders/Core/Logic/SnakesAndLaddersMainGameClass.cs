namespace SnakesAndLadders.Core.Logic;
[SingletonGame]
public class SnakesAndLaddersMainGameClass : BasicGameClass<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>
    , ICommonMultiplayer<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>
    , IMoveNM
{
    public SnakesAndLaddersMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        SnakesAndLaddersVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem> roll,
        GameBoardProcesses gameBoard1,
        SnakesAndLaddersGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _test = test;
        _model = model;
        Roll = roll;
        GameBoard1 = gameBoard1;
        _toast = toast;
        Roll.AfterRollingAsync = AfterRollingAsync;
        roll.CurrentPlayer = GetCurrentPlayer;
        GameBoard1.CurrentPlayer = GetCurrentPlayer;
    }
    private SnakesAndLaddersPlayerItem GetCurrentPlayer() => SingleInfo!;
    internal StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem> Roll { get; set; }
    internal GameBoardProcesses GameBoard1 { get; set; }
    private readonly TestOptions _test;
    private readonly SnakesAndLaddersVMData _model;
    private readonly IToast _toast;
    public override Task FinishGetSavedAsync()
    {
        _model.LoadCup(SaveRoot, true);
        SaveRoot!.DiceList.MainContainer = MainContainer;
        _model.Cup!.CanShowDice = SaveRoot.HasRolled;
        var player = PlayerList.GetWhoPlayer();
        GameBoard1.UpdateSpaceFromAutoResume(player);
        return Task.CompletedTask;
    }
    public async Task MakeMoveAsync(int space)
    {
        if (SingleInfo!.CanSendMessage(BasicData!) == true)
        {
            await Network!.SendMoveAsync(space);
        }
        await GameBoard1!.MakeMoveAsync(space);
        if (_test.NoAnimations == false)
        {
            await Delay!.DelayMilli(800);
        }
        if (GameBoard1.IsGameOver() == true)
        {
            await ShowWinAsync();
            return;
        }
        await EndTurnAsync();
    }
    public async Task MoveReceivedAsync(string data)
    {
        await MakeMoveAsync(int.Parse(data));
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (_test.NoAnimations == false)
        {
            await Delay!.DelayMilli(200);
        }
        if (SaveRoot!.HasRolled == false)
        {
            await Roll.RollDiceAsync();
            return;
        }
        int NewSpace = SingleInfo!.SpaceNumber + _model.Cup!.ValueOfOnlyDice;
        await MakeMoveAsync(NewSpace);
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        _model.LoadCup(SaveRoot!, false);
        SaveRoot!.DiceList.MainContainer = MainContainer;
        SaveRoot!.ImmediatelyStartTurn = true;
        PlayerList!.ForEach(thisPlayer => thisPlayer.SpaceNumber = 0);
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn();
        SaveRoot!.HasRolled = false;
        _model.Cup!.CanShowDice = false;
        GameBoard1!.NewTurn(this);
        await ContinueTurnAsync();
    }
    public async Task AfterRollingAsync()
    {
        SaveRoot!.HasRolled = true;
        if (GameBoard1!.HasValidMove() == false)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _toast.ShowInfoToast("No moves available.  Therefore, the turn will end");
            }
            await EndTurnAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    public Task FinishStartAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        GameBoard1!.NewTurn(this);
        return Task.CompletedTask;
    }
}