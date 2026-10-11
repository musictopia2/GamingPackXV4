namespace Mastermind.Core.ViewModels;
public class MastermindShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => false;
    protected override bool AutoStartNewGame => false;
    protected override bool HasPostGame => true;
}