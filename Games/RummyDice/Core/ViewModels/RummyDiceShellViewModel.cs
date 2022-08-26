namespace RummyDice.Core.ViewModels;
public class RummyDiceShellViewModel : BasicMultiplayerShellViewModel<RummyDicePlayerItem>
{
    public RummyDiceShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<RummyDiceMainViewModel>();
        return model;
    }
}