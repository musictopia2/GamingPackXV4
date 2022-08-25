namespace Mastermind.Core.ViewModels;
public class MastermindShellViewModel : SinglePlayerShellViewModel
{
    public MastermindShellViewModel(IGamePackageResolver mainContainer,
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
    public IScreen? SolutionScreen { get; set; }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<MastermindMainViewModel>();
        return model;
    }
    protected override async Task NewGameRequestedAsync()
    {
        if (OpeningScreen == null)
        {
            throw new CustomBasicException("There was no opening screen.  Rethink");
        }
        if (SolutionScreen != null)
        {
            await CloseSpecificChildAsync(SolutionScreen);
            SolutionScreen = null;
        }
        await CloseSpecificChildAsync(OpeningScreen);
    }
    protected override async Task GameOverScreenAsync()
    {
        SolutionScreen = MainContainer.Resolve<SolutionViewModel>();
        //try to clear.

        //IMainScreen temps = MainContainer.Resolve<MastermindMainViewModel>();
        //await temps.TryCloseAsync();
        await LoadScreenAsync(SolutionScreen);
    }
    protected override async Task OpenStartingScreensAsync()
    {
        OpeningScreen = MainContainer.Resolve<MastermindOpeningViewModel>(); //i think has to be this way so its fresh everytime.
        await LoadScreenAsync(OpeningScreen); //try this way.
        await ShowNewGameAsync();
        FinishInit();
    }
}