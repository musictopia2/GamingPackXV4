namespace Bingo.Core.ViewModels;
public class BingoShellViewModel : BasicMultiplayerShellViewModel<BingoPlayerItem>
{
    public BingoShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<BingoMainViewModel>();
        return model;
    }
}