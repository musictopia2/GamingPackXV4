namespace Backgammon.Core.ViewModels;
public class BackgammonShellViewModel : BasicBoardGamesShellViewModel<BackgammonPlayerItem>
{
    public BackgammonShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<BackgammonMainViewModel>();
        return model;
    }
}