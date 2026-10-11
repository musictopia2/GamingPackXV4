namespace BuncoDiceGame.Core.ViewModels;
public partial class BuncoDiceGameShellViewModel : SinglePlayerShellViewModel,
     IHandleAsync<ChoseNewRoundEventModel>,
        IHandleAsync<EndGameEventModel>,
        IHandleAsync<BuncoNewRoundEventModel>
{
    public BuncoDiceGameShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
        
    }
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    protected override bool AutoStartNewGame => true;
    public IScreen? TempScreen { get; set; }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<BuncoDiceGameMainViewModel>();

        return model;
    }
    async Task IHandleAsync<ChoseNewRoundEventModel>.HandleAsync(ChoseNewRoundEventModel message)
    {
        if (TempScreen == null)
        {
            throw new CustomBasicException("No screen was set up to show you chose new round.  Rethink");
        }
        await CloseSpecificChildAsync(TempScreen);
        TempScreen = null;
    }
    Task IHandleAsync<EndGameEventModel>.HandleAsync(EndGameEventModel message)
    {
        if (TempScreen != null)
        {
            throw new CustomBasicException("The screen was never closed out.  Rethink");
        }
        TempScreen = MainContainer.Resolve<EndGameViewModel>();
        return LoadScreenAsync(TempScreen);
    }
    protected override async Task GameOverScreenAsync()
    {
        if (TempScreen == null)
        {
            throw new CustomBasicException("Must have the end screen first.  Rethink");
        }
        await CloseSpecificChildAsync(TempScreen);
        TempScreen = null;
    }
    Task IHandleAsync<BuncoNewRoundEventModel>.HandleAsync(BuncoNewRoundEventModel message)
    {
        if (TempScreen != null)
        {
            throw new CustomBasicException("The screen was never closed out.  Rethink");
        }

        //var tt = MainContainer.Resolve<ChoseNewRoundEventModel>();
        TempScreen = MainContainer.Resolve<BuncoNewRoundViewModel>();
        return LoadScreenAsync(TempScreen);
    }
}