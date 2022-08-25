namespace SpiderSolitaire.Core.ViewModels;
public class SpiderSolitaireShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    protected override bool AutoStartNewGame => false;
    public SpiderSolitaireShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<SpiderSolitaireMainViewModel>();
        return model;
    }
    public IScreen? OpeningScreen { get; set; }
    protected override async Task NewGameRequestedAsync()
    {
        if (OpeningScreen == null)
        {
            throw new CustomBasicException("There was no opening screen.  Rethink");
        }
        RegularSimpleCard.ClearSavedList();
        await CloseSpecificChildAsync(OpeningScreen);
    }
    protected override async Task OpenStartingScreensAsync()
    {
        OpeningScreen = MainContainer.Resolve<SpiderSolitaireOpeningViewModel>(); //i think has to be this way so its fresh everytime.
        await LoadScreenAsync(OpeningScreen); //try this way.
        await ShowNewGameAsync();
        FinishInit();
    }
}