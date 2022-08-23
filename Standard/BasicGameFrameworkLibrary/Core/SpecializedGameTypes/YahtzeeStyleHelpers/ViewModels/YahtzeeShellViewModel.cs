namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;

public class YahtzeeShellViewModel<D> : BasicMultiplayerShellViewModel<YahtzeePlayerItem<D>>
    where D : SimpleDice, new()
{
    public YahtzeeShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast
        ) : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
    }
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