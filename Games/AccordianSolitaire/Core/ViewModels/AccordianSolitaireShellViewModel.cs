namespace AccordianSolitaire.Core.ViewModels;
public class AccordianSolitaireShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
    public AccordianSolitaireShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<AccordianSolitaireMainViewModel>();
        return model;
    }
}