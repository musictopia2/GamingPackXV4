namespace LifeBoardGame.Core.ViewModels;
public partial class LifeBoardGameShellViewModel : BasicBoardGamesShellViewModel<LifeBoardGamePlayerItem>
    , IHandleAsync<GenderEventModel>, IHandleAsync<StartEventModel>
{
    private readonly IMessageBox _message;
    public LifeBoardGameShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast,
        IMessageBox message
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        _message = message;
    }
    protected override bool CanOpenMainAfterColors => false;
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<LifeBoardGameMainViewModel>();
        return model;
    }
    public ChooseGenderViewModel? GenderScreen { get; set; }
    protected override async Task GetStartingScreenAsync()
    {
        LifeBoardGameSaveInfo saveRoot = MainContainer.Resolve<LifeBoardGameSaveInfo>();
        if (saveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
        {
            await LoadGenderAsync();
            return;
        }
        await base.GetStartingScreenAsync();
    }
    private async Task LoadGenderAsync()
    {
        if (GenderScreen != null)
        {
            throw new CustomBasicException("Cannot load gender because its already there.  Rethink");
        }
        GenderScreen = MainContainer.Resolve<ChooseGenderViewModel>();
        await LoadScreenAsync(GenderScreen);
    }
    async Task IHandleAsync<GenderEventModel>.HandleAsync(GenderEventModel message)
    {
        await LoadGenderAsync();
    }
    async Task IHandleAsync<StartEventModel>.HandleAsync(StartEventModel message)
    {
        if (GenderScreen is not null)
        {
            await CloseSpecificChildAsync(GenderScreen);
        }
        GenderScreen = null;
        LifeBoardGameMainGameClass game = MainContainer.Resolve<LifeBoardGameMainGameClass>();
        game.SaveRoot.GameStatus = EnumWhatStatus.NeedChooseFirstOption; //belongs here.
        await StartNewGameAsync();
        await game.AfterChoosingGenderAsync();
    }
}