namespace CribbagePatience.Core.ViewModels;
public class CribbagePatienceShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    public CribbagePatienceShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<CribbagePatienceMainViewModel>();
        return model;
    }
}