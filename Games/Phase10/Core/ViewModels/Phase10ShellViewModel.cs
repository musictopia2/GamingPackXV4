namespace Phase10.Core.ViewModels;
public class Phase10ShellViewModel : BasicMultiplayerShellViewModel<Phase10PlayerItem>
{
    public Phase10ShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<Phase10MainViewModel>();
        return model;
    }
}