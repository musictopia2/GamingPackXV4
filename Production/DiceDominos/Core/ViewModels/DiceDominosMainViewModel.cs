namespace DiceDominos.Core.ViewModels;
[InstanceGame]
public class DiceDominosMainViewModel(CommandContainer commandContainer,
        DiceDominosMainGameClass mainGame,
        DiceDominosVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        DiceDominosGameContainer gameContainer,
        GameBoardCP gameBoard,
        IEventAggregator aggregator,
        IToast toast) : DiceGamesVM<SimpleDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public DiceDominosVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        gameContainer.DominoClickedAsync = DominoClickedAsync;
        gameBoard.SendEnableProcesses(this, () => mainGame.SaveRoot.HasRolled);
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        gameContainer.DominoClickedAsync = null;
        return base.CloseAsync();
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public GameBoardCP GameBoard => gameBoard;
    public PlayerCollection<DiceDominosPlayerItem> PlayerList => mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        if (mainGame!.SaveRoot!.HasRolled == false || mainGame.SaveRoot.DidHold == true)
        {
            return false;
        }
        return true;
    }
    public override bool CanEndTurn()
    {
        return mainGame!.SaveRoot!.HasRolled;
    }
    public override bool CanRollDice()
    {
        return !mainGame!.SaveRoot!.HasRolled;
    }
    private async Task DominoClickedAsync(SimpleDominoInfo thisDomino)
    {
        if (GameBoard.IsValidMove(thisDomino.Deck) == false)
        {
            toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        await mainGame.MakeMoveAsync(thisDomino.Deck);
    }
}