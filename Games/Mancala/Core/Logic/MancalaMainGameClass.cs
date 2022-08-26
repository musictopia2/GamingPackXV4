namespace Mancala.Core.Logic;
[SingletonGame]
public class MancalaMainGameClass : BasicGameClass<MancalaPlayerItem, MancalaSaveInfo>
    , ICommonMultiplayer<MancalaPlayerItem, MancalaSaveInfo>
    , IMoveNM
{
    public MancalaMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        MancalaVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        GameBoardProcesses gameBoard,
        MancalaGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _model = model;
        _command = command;
        _gameBoard = gameBoard;
        _gameBoard.PlayerList = GetPlayerList;
        _gameBoard.SingleInfo = GetCurrentPlayer;
        _gameBoard.SetCurrentPlayer = ((x) => SingleInfo = x);
        _gameBoard.EndTurnAsync = EndTurnAsync;
        _gameBoard.WhoTurn = GetWhoTurn;
        _gameBoard.ContinueTurnAsync = ContinueTurnAsync;
        _gameBoard.ShowWinAsync = ShowWinAsync;
        _gameBoard.ShowTieAsync = ShowTieAsync;
        _gameBoard.SaveRoot = GetSaveInfo;
        _gameBoard.CreateSpaceList(); //maybe this now.
    }
    private MancalaSaveInfo GetSaveInfo() => SaveRoot;
    private int GetWhoTurn() => WhoTurn;
    private MancalaPlayerItem GetCurrentPlayer() => SingleInfo!;
    private PlayerCollection<MancalaPlayerItem> GetPlayerList() => PlayerList;
    private readonly MancalaVMData _model;
    private readonly CommandContainer _command;
    private readonly GameBoardProcesses _gameBoard;
    public void OpenMove()
    {
        _model.Instructions = "Waiting for the move results";
        _model.PiecesAtStart = 0;
        _model.PiecesLeft = 0;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        SingleInfo = PlayerList!.GetWhoPlayer();
        _gameBoard.LoadSavedBoard();
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
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        _gameBoard.ClearBoard();
        SaveRoot!.IsStart = true; //needs this too.
        await FinishUpAsync(isBeginning);
    }
    public override async Task ContinueTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        this.ShowTurn();
        if (SaveRoot!.IsStart == true)
        {
            await _gameBoard.StartNewTurnAsync();
        }
        else
        {
            ResetMove();
            _model.Instructions = "Make Move";
            _gameBoard.RepaintBoard();
            await base.ContinueTurnAsync();
        }
    }
    public override async Task EndTurnAsync()
    {
        ResetMove();
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        SaveRoot!.IsStart = true;
        _command.ManuelFinish = true;
        await ContinueTurnAsync(); //hopefully this simple.
    }
    private void ResetMove()
    {
        _model.SpaceSelected = 0;
        _model.SpaceStarted = 0;
        _gameBoard.Reset();
    }
    public override Task StartNewTurnAsync()
    {
        return Task.CompletedTask;
    }
    async Task IMoveNM.MoveReceivedAsync(string data)
    {
        OpenMove();
        await _gameBoard.AnimateMoveAsync(int.Parse(data));
    }
}