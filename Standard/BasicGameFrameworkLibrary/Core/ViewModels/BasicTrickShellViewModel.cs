namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract class BasicTrickShellViewModel<P>(IGamePackageResolver mainContainer,
    CommandContainer container,
    IGameInfo gameData,
    BasicData basicData,
    IMultiplayerSaveState save,
    TestOptions test,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicMultiplayerShellViewModel<P>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    where P : class, IPlayerItem, new()
{
    protected override BasicList<Type> GetAdditionalObjectsToReset()
    {
        return new BasicList<Type>() { typeof(IAdvancedTrickProcesses) };
    }
}