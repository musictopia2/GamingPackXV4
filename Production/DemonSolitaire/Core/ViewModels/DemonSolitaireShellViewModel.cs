namespace DemonSolitaire.Core.ViewModels;
public class DemonSolitaireShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, ISaveSinglePlayerClass saves, IEventAggregator aggregator) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => true;
}