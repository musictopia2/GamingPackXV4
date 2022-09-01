namespace Spades2Player.Core.ViewModels;
public class Spades2PlayerShellViewModel : BasicTrickShellViewModel<Spades2PlayerPlayerItem>
{
    public Spades2PlayerShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<Spades2PlayerMainViewModel>();
        return model;
    }
}