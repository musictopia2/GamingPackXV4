namespace Minesweeper.Core.ViewModels;
public class MinesweeperShellViewModel : SinglePlayerShellViewModel
{
    public MinesweeperShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override bool AlwaysNewGame => false;
    protected override bool AutoStartNewGame => false;
    public IScreen? OpeningScreen { get; set; }
    protected override async Task OpenStartingScreensAsync()
    {
        OpeningScreen = MainContainer.Resolve<MinesweeperOpeningViewModel>(); //i think has to be this way so its fresh everytime.
        await LoadScreenAsync(OpeningScreen); //try this way.
        await ShowNewGameAsync();
        FinishInit();
    }
    protected override Task NewGameRequestedAsync()
    {
        if (OpeningScreen == null)
        {
            throw new CustomBasicException("There was no opening screen.  Rethink");
        }
        return CloseSpecificChildAsync(OpeningScreen);
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<MinesweeperMainViewModel>();
        return model;
    }
}