namespace Poker.Core.ViewModels;
public class PokerShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    public PokerShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<PokerMainViewModel>();
        return model;
    }
}