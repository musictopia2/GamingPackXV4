namespace GermanWhist.Core.ViewModels;
public class GermanWhistShellViewModel : BasicTrickShellViewModel<GermanWhistPlayerItem>
{
    public GermanWhistShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<GermanWhistMainViewModel>();
        return model;
    }
}