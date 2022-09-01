namespace Pinochle2Player.Core.ViewModels;
public class Pinochle2PlayerShellViewModel : BasicTrickShellViewModel<Pinochle2PlayerPlayerItem>
{
    public Pinochle2PlayerShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<Pinochle2PlayerMainViewModel>();
        return model;
    }
}