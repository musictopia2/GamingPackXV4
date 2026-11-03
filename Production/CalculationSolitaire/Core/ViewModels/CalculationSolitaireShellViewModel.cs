namespace CalculationSolitaire.Core.ViewModels;
public class CalculationSolitaireShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, ISaveSinglePlayerClass saves, IEventAggregator aggregator) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => true;
}