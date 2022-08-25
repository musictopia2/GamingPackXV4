namespace ClockSolitaire.Core.ViewModels;
public class ClockSolitaireShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    public ClockSolitaireShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<ClockSolitaireMainViewModel>();
        return model;
    }
}