namespace DiceDominos.Core.ViewModels;
[InstanceGame]
public class DiceDominosMainViewModel : DiceGamesVM<SimpleDice>
{
    private readonly DiceDominosMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public DiceDominosVMData VMData { get; set; }
    public DiceDominosMainViewModel(CommandContainer commandContainer,
        DiceDominosMainGameClass mainGame,
        DiceDominosVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        DiceDominosGameContainer gameContainer,
        GameBoardCP gameBoard,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        GameBoard = gameBoard;
        _toast = toast;
        VMData = viewModel;
        gameContainer.DominoClickedAsync = DominoClickedAsync;
        gameBoard.SendEnableProcesses(this, (() => _mainGame.SaveRoot.HasRolled));
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public GameBoardCP GameBoard { get; }
    public PlayerCollection<DiceDominosPlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        if (_mainGame!.SaveRoot!.HasRolled == false || _mainGame.SaveRoot.DidHold == true)
        {
            return false;
        }
        return true;
    }
    public override bool CanEndTurn()
    {
        return _mainGame!.SaveRoot!.HasRolled;
    }
    public override bool CanRollDice()
    {
        return !_mainGame!.SaveRoot!.HasRolled;
    }
    private async Task DominoClickedAsync(SimpleDominoInfo thisDomino)
    {
        if (GameBoard.IsValidMove(thisDomino.Deck) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        await _mainGame.MakeMoveAsync(thisDomino.Deck);
    }
}