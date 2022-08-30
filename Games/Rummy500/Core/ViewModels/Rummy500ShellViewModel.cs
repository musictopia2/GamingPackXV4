namespace Rummy500.Core.ViewModels;
public class Rummy500ShellViewModel : BasicMultiplayerShellViewModel<Rummy500PlayerItem>
{
    public Rummy500ShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<Rummy500MainViewModel>();
        return model;
    }
}