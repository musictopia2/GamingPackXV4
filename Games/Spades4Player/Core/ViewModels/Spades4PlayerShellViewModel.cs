namespace Spades4Player.Core.ViewModels;
public class Spades4PlayerShellViewModel : BasicTrickShellViewModel<Spades4PlayerPlayerItem>
{
    public Spades4PlayerShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<Spades4PlayerMainViewModel>();
        return model;
    }
}