namespace RaglanSolitaire.Core.ViewModels;
public class RaglanSolitaireShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
    public RaglanSolitaireShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<RaglanSolitaireMainViewModel>();
        return model;
    }
}