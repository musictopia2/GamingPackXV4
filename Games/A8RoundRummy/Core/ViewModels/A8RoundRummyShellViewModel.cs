namespace A8RoundRummy.Core.ViewModels;
public class A8RoundRummyShellViewModel : BasicMultiplayerShellViewModel<A8RoundRummyPlayerItem>
{
    public A8RoundRummyShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<A8RoundRummyMainViewModel>();
        return model;
    }
}