namespace FillOrBust.Core.ViewModels;
public class FillOrBustShellViewModel : BasicMultiplayerShellViewModel<FillOrBustPlayerItem>
{
    public FillOrBustShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<FillOrBustMainViewModel>();
        return model;
    }
}