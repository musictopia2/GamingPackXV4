namespace Payday.Core.ViewModels;
public class PaydayShellViewModel(IGamePackageResolver mainContainer,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicBoardGamesShellViewModel<PaydayPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<PaydayMainViewModel>();
        return model;
    }
}