namespace ThreeLetterFun.Core.ViewModels;
public partial class ThreeLetterFunShellViewModel : BasicMultiplayerShellViewModel<ThreeLetterFunPlayerItem>, IHandleAsync<NextScreenEventModel>
{
    private readonly ISystemError _error;
    public static EnumTestCategory TestMode { get; set; } = EnumTestCategory.None;
    public ThreeLetterFunShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast,
        ISystemError error
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        _error = error;
    }
    protected override async Task ActivateAsync()
    {
        if (BasicData.MultiPlayer == true)
        {
            _error.ShowSystemError("Needs to rethink multiplayer."); //well see.
            return;
        }
        await base.ActivateAsync();
        if (TestMode != EnumTestCategory.None && BasicData.GamePackageMode == EnumGamePackageMode.Production)
        {
            throw new CustomBasicException("Cannot have a test mode for screens because its in production");
        }
        if (TestMode != EnumTestCategory.None)
        {
            IScreen screen;
            switch (TestMode)
            {
                case EnumTestCategory.FirstOption:
                    FirstScreen = MainContainer.Resolve<FirstOptionViewModel>();
                    screen = FirstScreen;
                    break;
                case EnumTestCategory.CardsPlayer:
                    CardsScreen = MainContainer.Resolve<CardsPlayerViewModel>();
                    screen = CardsScreen;
                    break;
                case EnumTestCategory.Advanced:
                    AdvancedScreen = MainContainer.Resolve<AdvancedOptionsViewModel>();
                    screen = AdvancedScreen;
                    break;
                default:
                    throw new CustomBasicException("Rethink");
            }
            await LoadScreenAsync(screen);
        }
    }
    protected override bool CanStartWithOpenScreen => TestMode == EnumTestCategory.None;
    public FirstOptionViewModel? FirstScreen { get; set; }
    public AdvancedOptionsViewModel? AdvancedScreen { get; set; }
    public CardsPlayerViewModel? CardsScreen { get; set; }
    //for each step, will close the screens it was up to.
    protected override BasicList<Type> GetAdditionalObjectsToReset()
    {
        return new BasicList<Type>()
            {
                typeof(GenericCardShuffler<ThreeLetterFunCardData>)
            };
    }
    private async Task CloseStartingScreensAsync()
    {
        if (FirstScreen != null)
        {
            await CloseSpecificChildAsync(FirstScreen);
            FirstScreen = null;
            return;
        }
        if (AdvancedScreen != null)
        {
            await CloseSpecificChildAsync(AdvancedScreen);
            AdvancedScreen = null;
        }
        if (CardsScreen != null)
        {
            await CloseSpecificChildAsync(CardsScreen);
            CardsScreen = null;
        }
    }
    protected override async Task GetStartingScreenAsync()
    {
        FirstScreen = MainContainer.Resolve<FirstOptionViewModel>();
        await LoadScreenAsync(FirstScreen);
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<ThreeLetterFunMainViewModel>();
        return model;
    }
    async Task IHandleAsync<NextScreenEventModel>.HandleAsync(NextScreenEventModel message)
    {
        await CloseStartingScreensAsync();
        switch (message.Screen)
        {
            case EnumNextScreen.Advanced:
                AdvancedScreen = MainContainer.Resolve<AdvancedOptionsViewModel>();
                await LoadScreenAsync(AdvancedScreen);
                break;
            case EnumNextScreen.Cards:
                CardsScreen = MainContainer.Resolve<CardsPlayerViewModel>();
                await LoadScreenAsync(CardsScreen);
                break;
            case EnumNextScreen.Finished:
                await StartNewGameAsync();
                break;
            default:
                throw new CustomBasicException("Next screen not supported");
        }
    }
}