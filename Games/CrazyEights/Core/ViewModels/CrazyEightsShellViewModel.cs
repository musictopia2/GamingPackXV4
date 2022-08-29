namespace CrazyEights.Core.ViewModels;
public class CrazyEightsShellViewModel : BasicMultiplayerShellViewModel<CrazyEightsPlayerItem>
{
    public CrazyEightsShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<CrazyEightsMainViewModel>();
        return model;
    }
}