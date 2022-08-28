namespace A21DiceGame.Core.ViewModels;
public class A21DiceGameShellViewModel : BasicMultiplayerShellViewModel<A21DiceGamePlayerItem>
{
    public A21DiceGameShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<A21DiceGameMainViewModel>();
        return model;
    }
}