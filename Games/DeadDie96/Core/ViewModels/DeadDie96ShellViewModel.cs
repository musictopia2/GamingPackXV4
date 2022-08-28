namespace DeadDie96.Core.ViewModels;
public class DeadDie96ShellViewModel : BasicMultiplayerShellViewModel<DeadDie96PlayerItem>
{
    public DeadDie96ShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<DeadDie96MainViewModel>();
        return model;
    }
}