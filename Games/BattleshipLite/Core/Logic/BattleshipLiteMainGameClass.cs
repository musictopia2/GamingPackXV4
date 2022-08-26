namespace BattleshipLite.Core.Logic;
[SingletonGame]
public class BattleshipLiteMainGameClass : BasicGameClass<BattleshipLitePlayerItem, BattleshipLiteSaveInfo>
    , ICommonMultiplayer<BattleshipLitePlayerItem, BattleshipLiteSaveInfo>
{
    public BattleshipLiteMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        BattleshipLiteVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        BattleshipLiteGameContainer gameContainer,
        BattleshipBoardClass board,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _board = board;
    }
    private readonly BattleshipBoardClass _board;
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
        ShipInfo ship;
        if (SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            //the computer will place their ships.
            await Delay!.DelayMilli(50);
            var firsts = SingleInfo!.Ships.Where(x => x.WasShip == false).ToBasicList();
            ship = firsts.GetRandomItem();
            await _board.MakeMoveAsync(ship);
            return;
        }
        await Delay!.DelayMilli(500);
        var nexts = SingleInfo!.Ships.Where(x => x.ShipStatus == EnumShipStatus.None).ToBasicList();
        ship = nexts.GetRandomItem();
        await _board.MakeMoveAsync(ship);
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        SaveRoot.GameStatus = EnumGameStatus.PlacingShips;
        foreach (var player in PlayerList)
        {
            foreach (var ship in player.Ships)
            {
                ship.ClearSpace(); //you have to clear everything to start again.
            }
        }
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        await FinishUpAsync(isBeginning);
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn(); //anything else is below.

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override async Task EndTurnAsync()
    {
        _board.PossibleChangeStatus();
        bool rets = _board.IsGameOver();
        if (rets)
        {
            await ShowWinAsync();
            return;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
}