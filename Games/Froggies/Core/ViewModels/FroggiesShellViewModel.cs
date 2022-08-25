namespace Froggies.Core.ViewModels;
public class FroggiesShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    protected override bool AutoStartNewGame => false;
    public IScreen? OpeningScreen { get; set; }
    public FroggiesShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override Task GameOverScreenAsync()
    {
        throw new CustomBasicException("There is no option for new game.  Rethink");
    }
    protected override async Task NewGameRequestedAsync()
    {
        if (OpeningScreen == null)
        {
            throw new CustomBasicException("There was no opening screen.  Rethink");
        }
        await CloseSpecificChildAsync(OpeningScreen);
        OpeningScreen = null;
    }
    protected override async Task OpenStartingScreensAsync()
    {
        OpeningScreen = MainContainer.Resolve<FroggiesOpeningViewModel>();
        await LoadScreenAsync(OpeningScreen);
        await ShowNewGameAsync();
        FinishInit();
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<FroggiesMainViewModel>();
        return model;
    }
}