namespace Battleship.Core.Logic;
[SingletonGame]
public class BattleshipMainGameClass(IGamePackageResolver resolver,
    IEventAggregator aggregator,
    BasicData basic,
    TestOptions test,
    BattleshipVMData model,
    IMultiplayerSaveState state,
    IAsyncDelayer delay,
    CommandContainer command,
    BattleshipGameContainer gameContainer,
     GameBoardCP gameBoard1,
     ShipControlCP ships,
    ISystemError error,
    IToast toast
        ) : BasicGameClass<BattleshipPlayerItem, BattleshipSaveInfo>(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast),
    IMiscDataNM, IMoveNM, ISerializable,
    ICommonMultiplayer<BattleshipPlayerItem, BattleshipSaveInfo>
{
    internal GameBoardCP GameBoard1 { get; set; } = gameBoard1;
    internal ShipControlCP Ships { get; set; } = ships;
    public Vector CurrentSpace { get; set; }
    private BattleshipComputerAI? _ai;
    public EnumStatusList StatusOfGame { get; set; } //has to be part of game class.
    public override Task FinishGetSavedAsync()
    {
        if (BasicData.MultiPlayer == false)
        {
            throw new CustomBasicException("Only multiplayer should have received this since no autoresume");
        }
        if (BasicData.Client == false)
        {
            throw new CustomBasicException("The host should never get this");
        }
        LoadControls();
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        if (BasicData.MultiPlayer == false)
        {
            _ai = MainContainer.Resolve<BattleshipComputerAI>();
        }
        StatusOfGame = EnumStatusList.PlacingShips;
        Ships.ClearBoard();
        GameBoard1.ClearBoard();
        model.ShipDirectionsVisible = true; //hopefully this is it (?)
        IsLoaded = true; //i think needs to be here.
    }
    public async override Task ContinueTurnAsync()
    {
        if (StatusOfGame == EnumStatusList.PlacingShips)
        {
            model.NormalTurn = "None";
            model.Status = "Please place your ships by clicking on the ship and cliking on the grid where you want to place them.";
            CommandContainer.ManuelFinish = false;
            CommandContainer.IsExecuting = false; //at this point, show its done so you can do what you need to do in order to choose ships.
            if (BasicData.MultiPlayer == false)
            {
                return;
            }
            Network!.IsEnabled = false; //has to show IsEnabled as false so you won't receive the message until after placing ships.
            return;
        }
        await base.ContinueTurnAsync();
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        BattleshipMainViewModel.GamesPlayed++;
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        await FinishUpAsync(isBeginning);
    }
    public async Task MakeMoveAsync(Vector Space, EnumWhatHit HitType)
    {
        if (HitType == EnumWhatHit.None)
        {
            throw new CustomBasicException("Must be hit or miss");
        }
        bool IsSelf;
        IsSelf = SingleInfo!.PlayerCategory == EnumPlayerCategory.Self;
        if (IsSelf == true)
        {
            GameBoard1!.MarkField(Space, HitType);
            if (HitType == EnumWhatHit.Hit && GameBoard1.HumanWon() == true)
            {
                await ShowWinAsync();
                return;
            }
        }
        else if (HitType == EnumWhatHit.Hit && Ships!.HasLost() == true)
        {
            await ShowWinAsync();
            return;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private async Task ComputerPlaceShipsAsync()
    {
        await _ai!.ComputerPlaceShipsAsync();
        await StartGameAsync();
    }
    public async Task HumanFinishedPlacingShipsAsync()
    {
        model.Status = "Waiting for other player to place their ships";
        model.ShipDirectionsVisible = false;
        CommandContainer.ManuelFinish = true; //now has to manually be done.
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendAllAsync("finishedships");
            Network!.IsEnabled = true;
            return;
        }
        await ComputerPlaceShipsAsync();
    }
    protected async override Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.3);
        }
        Vector space = _ai!.ComputerMove();
        FieldInfoCP ThisField = GameBoard1!.ComputerList![space];
        if (Ships!.HasHit(space))
        {
            ThisField.Hit = EnumWhatHit.Hit;
            _ai.MarkHit(space);
            await MakeMoveAsync(space, EnumWhatHit.Hit);
            return;
        }
        ThisField.Hit = EnumWhatHit.Miss;
        await MakeMoveAsync(space, EnumWhatHit.Miss);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "finishedships":
                await StartGameAsync();
                break;
            case "hit":
                await MakeMoveAsync(CurrentSpace, EnumWhatHit.Hit);
                break;
            case "miss":
                await MakeMoveAsync(CurrentSpace, EnumWhatHit.Miss);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    async Task IMoveNM.MoveReceivedAsync(string data)
    {
        Vector space = await js1.DeserializeObjectAsync<Vector>(data);
        bool hasHit = Ships!.HasHit(space);
        if (hasHit == true)
        {
            await Network!.SendAllAsync("hit");
            await MakeMoveAsync(space, EnumWhatHit.Hit);
        }
        else
        {
            await Network!.SendAllAsync("miss");
            await MakeMoveAsync(space, EnumWhatHit.Miss);
        }
    }
    public async Task StartGameAsync()
    {
        StatusOfGame = EnumStatusList.InGame;
        this.StartingStatus();
        if (BasicData!.MultiPlayer == false)
        {
            _ai!.StartNewGame();
        }
        GameBoard1!.StartGame();
        if (GameBoard1.HumanList!.Any(Items => Items.ShipNumber > 0) == false)
        {
            throw new CustomBasicException("Human ships was never marked.  Rethink");
        }
        await StartNewTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        CurrentSpace = new Vector();
        PrepStartTurn(); //i think
        await ContinueTurnAsync(); //i think
    }
}