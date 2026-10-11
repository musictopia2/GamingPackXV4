namespace SpiderSolitaire.Core.ViewModels;
public class SpiderSolitaireShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    protected override bool AutoStartNewGame => false;
}