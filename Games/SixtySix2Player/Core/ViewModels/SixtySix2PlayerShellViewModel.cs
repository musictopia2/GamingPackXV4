namespace SixtySix2Player.Core.ViewModels;
public class SixtySix2PlayerShellViewModel : BasicTrickShellViewModel<SixtySix2PlayerPlayerItem>
{
    public SixtySix2PlayerShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<SixtySix2PlayerMainViewModel>();
        return model;
    }
}