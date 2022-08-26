namespace LottoDominos.Core.ViewModels;
[InstanceGame]
public partial class LottoDominosMainViewModel : BasicMultiplayerMainVM, IHandleAsync<ChangeGameStatusEventModel>
{
    private readonly LottoDominosMainGameClass _mainGame; //if we don't need, delete.
    private readonly IGamePackageResolver _resolver;
    public LottoDominosVMData VMData { get; set; }
    public LottoDominosMainViewModel(CommandContainer commandContainer,
        LottoDominosMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        LottoDominosVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        _resolver = resolver;
        VMData = data;
    }
    public ChooseNumberViewModel? ChooseScreen { get; set; }
    public MainBoardViewModel? BoardScreen { get; set; } //you have to do properties for screens.
    public PlayerCollection<LottoDominosPlayerItem> GetPlayers => _mainGame.SaveRoot.PlayerList;
    protected override async Task ActivateAsync()
    {
        if (_mainGame.SaveRoot.GameStatus == EnumStatus.ChooseNumber)
        {
            ChooseScreen = _resolver.Resolve<ChooseNumberViewModel>();
            await LoadScreenAsync(ChooseScreen);
            return;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumStatus.NormalPlay)
        {
            BoardScreen = _resolver.Resolve<MainBoardViewModel>();
            await LoadScreenAsync(BoardScreen);
            return;
        }
        throw new CustomBasicException("Rethink because no status found");
    }
    protected override async Task TryCloseAsync()
    {
        if (ChooseScreen is not null)
        {
            await CloseSpecificChildAsync(ChooseScreen);
        }
        if (BoardScreen is not null)
        {
            await CloseSpecificChildAsync(BoardScreen);
        }
        await base.TryCloseAsync();
    }
    async Task IHandleAsync<ChangeGameStatusEventModel>.HandleAsync(ChangeGameStatusEventModel message)
    {
        if (_mainGame.SaveRoot.GameStatus != EnumStatus.NormalPlay)
        {
            throw new CustomBasicException("Only normal play is supported to change status.  Otherwise, rethink");
        }
        if (ChooseScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(ChooseScreen);
        ChooseScreen = null;
        BoardScreen = _resolver.Resolve<MainBoardViewModel>();
        await LoadScreenAsync(BoardScreen);
    }
}