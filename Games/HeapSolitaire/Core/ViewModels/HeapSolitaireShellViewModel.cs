namespace HeapSolitaire.Core.ViewModels;
public class HeapSolitaireShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
    public HeapSolitaireShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<HeapSolitaireMainViewModel>();
        return model;
    }
}