namespace BladesOfSteel.Core.ViewModels;
public class BladesOfSteelShellViewModel : BasicMultiplayerShellViewModel<BladesOfSteelPlayerItem>
{
    public BladesOfSteelShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        BladesOfSteelScreenDelegates delegates,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        delegates.ReloadFaceoffAsync = LoadFaceoffAsync;
        delegates.LoadMainGameAsync = LoadMainGameAsync;
    }
    public FaceoffViewModel? FaceoffScreen { get; set; }
    private async Task LoadMainGameAsync()
    {
        if (FaceoffScreen == null)
        {
            throw new CustomBasicException("Faceoff should have been loaded first.  Rethink");
        }
        await CloseSpecificChildAsync(FaceoffScreen);
        FaceoffScreen = null;
        MainVM = MainContainer.Resolve<BladesOfSteelMainViewModel>();
        await LoadScreenAsync(MainVM);
    }
    private async Task LoadFaceoffAsync()
    {
        if (MainVM != null)
        {
            await CloseSpecificChildAsync(MainVM);
            MainVM = null;
        }
        if (FaceoffScreen != null)
        {
            throw new CustomBasicException("Faceoff should be null when loading faceoffs");
        }
        FaceoffScreen = MainContainer.Resolve<FaceoffViewModel>();
        await LoadScreenAsync(FaceoffScreen);
    }
    protected override async Task StartNewGameAsync()
    {
        if (MainVM is not null)
        {
            await MainVM.TryCloseAsync();
        }
        BladesOfSteelGameContainer gameContainer = MainContainer.Resolve<BladesOfSteelGameContainer>();
        if (gameContainer.SaveRoot is null || gameContainer.SaveRoot.IsFaceOff)
        {
            if (FaceoffScreen != null)
            {
                await CloseSpecificChildAsync(FaceoffScreen);
                FaceoffScreen = null;
            }
            await LoadFaceoffAsync();
            return;
        }
        MainVM = MainContainer.Resolve<BladesOfSteelMainViewModel>();
        await LoadScreenAsync(MainVM); //try this way.
    }
    protected override IMainScreen GetMainViewModel()
    {
        throw new CustomBasicException("Something else should have happened instead of getting the main view model because of faceoffs");
    }
}