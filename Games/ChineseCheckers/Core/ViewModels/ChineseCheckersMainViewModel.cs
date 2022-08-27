namespace ChineseCheckers.Core.ViewModels;
[InstanceGame]
public class ChineseCheckersMainViewModel : BasicMultiplayerMainVM
{
    private readonly GameBoardProcesses _gameBoard;
    public ChineseCheckersVMData VMData { get; set; }
    public ChineseCheckersMainViewModel(CommandContainer commandContainer,
        ChineseCheckersMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        GameBoardProcesses gameBoard,
        IEventAggregator aggregator,
        ChineseCheckersVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _gameBoard = gameBoard;
        VMData = data;
    }
    public override bool CanEndTurn()
    {
        return _gameBoard.WillContinueTurn();
    }
}