namespace RollEm.Core.ViewModels;
[InstanceGame]
public class RollEmMainViewModel : DiceGamesVM<SimpleDice>
{
    private readonly RollEmMainGameClass _mainGame;
    public RollEmVMData VMData { get; set; }
    private readonly GameBoardProcesses _gameBoard;
    private readonly IToast _toast;
    private readonly BasicData _basicData;
    private readonly IEventAggregator _aggregator;
    public RollEmMainViewModel(CommandContainer commandContainer,
        RollEmMainGameClass mainGame,
        RollEmVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        RollEmGameContainer gameContainer,
        IEventAggregator aggregator,
        GameBoardProcesses gameBoard,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _gameBoard = gameBoard;
        _toast = toast;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        gameContainer.MakeMoveAsync = MakeMoveAsync;
        _basicData = basicData;
        _aggregator = aggregator;
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<RollEmPlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return false;
    }
    public override bool CanEndTurn()
    {
        return _mainGame!.SaveRoot!.GameStatus != EnumStatusList.NeedRoll;
    }
    public override bool CanRollDice()
    {
        return _mainGame!.SaveRoot!.GameStatus == EnumStatusList.NeedRoll;
    }
    private async Task MakeMoveAsync(int space)
    {
        if (_gameBoard.CanMakeMove(space) == false)
        {
            if (_gameBoard.HadRecent)
            {
                if (_basicData.MultiPlayer)
                {
                    await _mainGame.Network!.SendAllAsync("clearrecent");
                }
                _toast.ShowUserErrorToast("Illegal Move");
                _gameBoard.ClearRecent(true);
                await _mainGame.ContinueTurnAsync();
            }
            return;
        }
        await _mainGame.MakeMoveAsync(space);
    }
    private void CommandContainer_ExecutingChanged()
    {
        _aggregator.RepaintBoard();
    }
}