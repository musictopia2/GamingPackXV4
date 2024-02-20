namespace Risk.Core.ViewModels;
public class RiskShellViewModel : BasicBoardGamesShellViewModel<RiskPlayerItem>
{
    private readonly IMessageBox _message;
    public RiskShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast,
        IMessageBox message
        )
        : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
        _message = message;
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<RiskMainViewModel>();
        return model;
    }
}