namespace SinglePlayerMiscGames.Core.ViewModels;
public class SinglePlayerMiscGamesShellViewModel(IGamePackageResolver main,
    CommandContainer command,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(main, command, saves, aggregator)
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
}