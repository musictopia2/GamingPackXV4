namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract class BeginningColorShellViewModel<P>(IGamePackageResolver mainContainer,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast) : BasicMultiplayerShellViewModel<P>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    where P : class, IPlayerColorChoice, new()
{
    protected override EnumOpeningCategory OpeningCategory => EnumOpeningCategory.ChooseColor;




}
