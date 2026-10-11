namespace BakersDozenSolitaire.Core.ViewModels;
public class BakersDozenSolitaireShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, ISaveSinglePlayerClass saves, IEventAggregator aggregator) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => true;
}