namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
public class YahtzeeShellViewModel<D>(IGamePackageResolver mainContainer,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicMultiplayerShellViewModel<YahtzeePlayerItem<D>>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    where D : SimpleDice, new()
{
    protected override BasicList<Type> GetAdditionalObjectsToReset()
    {
        BasicList<Type> output = new()
    {
        typeof(IYahtzeeStyle),
        typeof(ScoreLogic),
        typeof(ScoreContainer),
        typeof(YahtzeeGameContainer<D>),
        typeof(YahtzeeEndRoundLogic<D>),
        typeof(YahtzeeMove<D>),
        typeof(YahtzeeVMData<D>)
    };
        return output;
    }
    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        Aggregator.Clear<SelectionChosenEventModel>();
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<YahtzeeMainViewModel<D>>();
        return model;
    }
}