namespace Candyland.Core.Logic;
[SingletonGame]
public class CandylandMainGameClass : BasicGameClass<CandylandPlayerItem, CandylandSaveInfo>
    , ICommonMultiplayer<CandylandPlayerItem, CandylandSaveInfo>
    , IMiscDataNM, IFinishStart
{
    public CandylandBoardProcesses GameBoard1;
    private readonly IToast _toast;
    public CandylandMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        CandylandVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        DrawShuffleClass<CandylandCardData, CandylandPlayerItem> shuffle,
        CandylandBoardProcesses gameBoard1,
        CandylandGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _test = test;
        _command = command;
        _shuffle = shuffle;
        GameBoard1 = gameBoard1;
        _toast = toast;
        _shuffle.CurrentPlayer = CurrentPlayer;
        _shuffle.AfterDrawingAsync = AfterDrawingAsync;
    }
    public async Task DrawAsync()
    {
        await _shuffle.DrawAsync();
    }
    private CandylandPlayerItem CurrentPlayer()
    {
        return SingleInfo!;
    }
    private readonly TestOptions _test;
    private readonly CommandContainer _command;
    private readonly DrawShuffleClass<CandylandCardData, CandylandPlayerItem> _shuffle;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        _shuffle.SaveRoot = SaveRoot;
        return Task.CompletedTask;
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        GameBoard1.LoadBoard();
        IsLoaded = true;
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        _shuffle.SaveRoot = SaveRoot;
        SaveRoot!.CurrentCard = new CandylandCardData();
        await _shuffle.FirstShuffleAsync(true);
        CandylandBoardProcesses.ClearBoard(this);
        await FinishUpAsync(isBeginning);
    }
    public async Task GameOverAsync()
    {
        SaveRoot!.CurrentCard!.Visible = false;
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.CanSendMessage(BasicData!) == true)
        {
            await Network!.SendAllAsync("castle");
        }
        await GameBoard1!.MakeMoveAsync(127, this);
        await ShowWinAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "space":
                await MakeMoveAsync(int.Parse(content));
                return;
            case "castle":
                await GameOverAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        SaveRoot!.DidDraw = false;
        await SaveStateAsync();
        SingleInfo = PlayerList!.GetWhoPlayer();
        GameBoard1!.NewTurn();
        await DrawAsync();
    }
    public async override Task EndTurnAsync()
    {
        _command.ManuelFinish = true;
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        this.ShowTurn();
        await StartNewTurnAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (_test.NoAnimations == false)
        {
            await Delay!.DelaySeconds(3);
        }
        if (GameBoard1!.IsValidMove(127, SaveRoot!.CurrentCard!.WhichCard, this, SaveRoot.CurrentCard.HowMany))
        {
            await GameOverAsync();
            return;
        }
        for (int x = 1; x <= 126; x++)
        {
            if (GameBoard1.IsValidMove(x, SaveRoot.CurrentCard!.WhichCard, this, SaveRoot.CurrentCard.HowMany))
            {
                await MakeMoveAsync(x);
                return;
            }
        }
        throw new CustomBasicException("The Computer Is Stuck");
    }
    public async Task MakeMoveAsync(int space)
    {
        if (space < 127)
        {
            if (SingleInfo!.CanSendMessage(BasicData!) == true)
            {
                await Network!.SendAllAsync("space", space);
            }
        }
        await GameBoard1!.MakeMoveAsync(space, this);
        if (CandylandBoardProcesses.WillMissNextTurn(this) == true)
        {
            SingleInfo!.MissNextTurn = true;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _toast.ShowWarningToast("Miss next turn for falling in a pit");
            }
            if (PlayerList.Any(xx => xx.MissNextTurn == true) == false)
            {
                throw new CustomBasicException("Did fall.  Find out what happened");
            }
        }
        if (space < 127)
        {
            await EndTurnAsync(); //decided to end turn automatically now.
        }
    }
    private bool _wasTemp;
    public async Task AfterDrawingAsync()
    {
        SaveRoot!.DidDraw = true;
        if (IsLoaded == false)
        {
            return;
        }
        if (_wasTemp == true)
        {
            _wasTemp = false;
            return;
        }
        await ContinueTurnAsync();
    }
    public async Task FinishStartAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo == null)
        {
            throw new CustomBasicException("Can't register new turn if we don't have a player yet");
        }
        GameBoard1!.NewTurn();

        if (SaveRoot!.DidDraw == false)
        {
            _wasTemp = true;
            await _shuffle!.DrawAsync();
            return;
        }
        SaveRoot.CurrentCard!.Visible = true;
        Aggregator.Publish(SaveRoot.CurrentCard);
    }
}