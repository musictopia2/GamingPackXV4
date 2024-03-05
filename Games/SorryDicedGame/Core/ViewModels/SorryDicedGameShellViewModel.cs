namespace SorryDicedGame.Core.ViewModels;
public class SorryDicedGameShellViewModel : BasicBoardGamesShellViewModel<SorryDicedGamePlayerItem>
{
    public SorryDicedGameShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<SorryDicedGameMainViewModel>();
        return model;
    }
}