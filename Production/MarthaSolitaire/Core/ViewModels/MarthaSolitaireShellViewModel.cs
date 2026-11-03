namespace MarthaSolitaire.Core.ViewModels;
public class MarthaSolitaireShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, ISaveSinglePlayerClass saves, IEventAggregator aggregator) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => true;
}