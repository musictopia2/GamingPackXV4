namespace ThinkTwice.Core.ViewModels;
public class ThinkTwiceShellViewModel : BasicMultiplayerShellViewModel<ThinkTwicePlayerItem>
{
    public ThinkTwiceShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<ThinkTwiceMainViewModel>();
        return model;
    }
}