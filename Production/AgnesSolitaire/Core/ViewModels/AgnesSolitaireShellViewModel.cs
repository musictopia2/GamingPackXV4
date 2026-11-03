namespace AgnesSolitaire.Core.ViewModels;
public class AgnesSolitaireShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, ISaveSinglePlayerClass saves, IEventAggregator aggregator) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => true;
}