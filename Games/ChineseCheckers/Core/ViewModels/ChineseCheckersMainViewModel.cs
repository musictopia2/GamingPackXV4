namespace ChineseCheckers.Core.ViewModels;
[InstanceGame]
public class ChineseCheckersMainViewModel(CommandContainer commandContainer,
        ChineseCheckersMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        GameBoardProcesses gameBoard,
        IEventAggregator aggregator,
        ChineseCheckersVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public ChineseCheckersVMData VMData { get; set; } = data;
    public override bool CanEndTurn()
    {
        return gameBoard.WillContinueTurn();
    }
}