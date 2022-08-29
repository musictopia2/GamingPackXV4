namespace PassOutDiceGame.Core.ViewModels;
[InstanceGame]
public class PassOutDiceGameMainViewModel : BoardDiceGameVM
{
    private readonly PassOutDiceGameMainGameClass _mainGame; //if we don't need, delete.
    public PassOutDiceGameVMData VMData { get; set; }
    private readonly GameBoardProcesses _gameBoard;
    public PassOutDiceGameMainViewModel(CommandContainer commandContainer,
        PassOutDiceGameMainGameClass mainGame,
        PassOutDiceGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        PassOutDiceGameGameContainer gameContainer,
        GameBoardProcesses gameBoard,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _gameBoard = gameBoard;
        gameContainer.MakeMoveAsync = MakeMoveAsync;
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<PassOutDiceGamePlayerItem> PlayerList => _mainGame.PlayerList;
    private async Task MakeMoveAsync(int space)
    {
        if (_gameBoard.IsValidMove(space) == false)
        {
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendMoveAsync(space);
        }
        await _mainGame.MakeMoveAsync(space);
    }
    public override bool CanRollDice()
    {
        return false;
    }
    public override async Task RollDiceAsync()
    {
        await base.RollDiceAsync();
    }
}