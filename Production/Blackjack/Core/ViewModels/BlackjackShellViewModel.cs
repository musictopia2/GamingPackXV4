namespace Blackjack.Core.ViewModels;
public class BlackjackShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(mainContainer, container, saves, aggregator)
{
    protected override bool AlwaysNewGame => false;
}