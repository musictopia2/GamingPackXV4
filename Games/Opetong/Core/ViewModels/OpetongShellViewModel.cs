namespace Opetong.Core.ViewModels;
public class OpetongShellViewModel : BasicMultiplayerShellViewModel<OpetongPlayerItem>
{
    public OpetongShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<OpetongMainViewModel>();
        return model;
    }
}