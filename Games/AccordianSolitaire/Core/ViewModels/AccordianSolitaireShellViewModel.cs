namespace AccordianSolitaire.Core.ViewModels;
public class AccordianSolitaireShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => true;
}