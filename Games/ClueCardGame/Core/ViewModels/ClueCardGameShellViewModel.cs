namespace ClueCardGame.Core.ViewModels;
public class ClueCardGameShellViewModel : BasicMultiplayerShellViewModel<ClueCardGamePlayerItem>
{
    public ClueCardGameShellViewModel(IGamePackageResolver mainContainer,
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
        var model = MainContainer.Resolve<ClueCardGameMainViewModel>();
        return model;
    }
}