namespace SinglePlayerCardGames.Core.ViewModels;
public class SinglePlayerCardGamesShellViewModel(IGamePackageResolver main,
    CommandContainer container,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(main, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
}