namespace LittleSpiderSolitaire.Core.ViewModels;
public class LittleSpiderSolitaireShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
    public LittleSpiderSolitaireShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<LittleSpiderSolitaireMainViewModel>();
        return model;
    }
}