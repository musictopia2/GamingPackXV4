namespace SolitaireBoardGame.Core.ViewModels;
public class SolitaireBoardGameShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
    public SolitaireBoardGameShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<SolitaireBoardGameMainViewModel>();
        return model;
    }
}