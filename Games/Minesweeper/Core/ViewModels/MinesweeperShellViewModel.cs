namespace Minesweeper.Core.ViewModels;
public class MinesweeperShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => false;
    protected override bool AutoStartNewGame => false;
}