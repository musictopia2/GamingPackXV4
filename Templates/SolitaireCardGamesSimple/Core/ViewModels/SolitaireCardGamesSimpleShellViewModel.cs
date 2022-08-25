namespace SolitaireCardGamesSimple.Core.ViewModels;
public class SolitaireCardGamesSimpleShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
    public SolitaireCardGamesSimpleShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<SolitaireCardGamesSimpleMainViewModel>();
        return model;
    }
}